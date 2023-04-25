using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.LiveStream.V1;
using Google.LongRunning;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace PallyCon
{
    class LiveStreamAPIClient
    {
        LivestreamServiceClient Client { get; }
        string ProjectId { get; }
        string LocationId { get; }
        public LiveStreamAPIClient(string projectId, string locationId)
        {
            Client = LivestreamServiceClient.Create();
            ProjectId = projectId;
            LocationId = locationId;
        }
        public async Task<Input> CreateInputAsync(string inputId)
        {
            CreateInputRequest request = new CreateInputRequest
            {
                ParentAsLocationName = LocationName.FromProjectLocation(ProjectId, LocationId),
                InputId = inputId,
                Input = new Input
                {
                    Type = Input.Types.Type.RtmpPush
                }
            };

            // Make the request.
            Operation<Input, OperationMetadata> response = await Client.CreateInputAsync(request);

            // Poll until the returned long-running operation is complete.
            Operation<Input, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

            // Retrieve the operation result.
            return completedResponse.Result;
        }
        public Input GetInput(string inputId)
        {
            GetInputRequest request = new GetInputRequest
            {
                InputName = InputName.FromProjectLocationInput(ProjectId, LocationId, inputId)
            };

            // Make the request.
            Input response = Client.GetInput(request);

            return response;
        }
        public async Task DeleteInputAsync(string inputId)
        {
            DeleteInputRequest request = new DeleteInputRequest
            {
                InputName = InputName.FromProjectLocationInput(ProjectId, LocationId, inputId)
            };

            // Make the request.
            Operation<Empty, OperationMetadata> response = await Client.DeleteInputAsync(request);

            // Poll until the returned long-running operation is complete.
            await response.PollUntilCompletedAsync();
        }
        public async Task<Channel> CreateChannelAsync(string channelId, string inputId, string outputUri, string secretVersion,
            RepeatedField<string> widevineMuxStreams, RepeatedField<string> playreadyMuxStreams, RepeatedField<string> fairplayMuxStreams)
        {
            InputAttachment inputAttachment = new InputAttachment
            {
                Key = "my-input",
                InputAsInputName = InputName.FromProjectLocationInput(ProjectId, LocationId, inputId)
            };

            // Elementary streams
            VideoStream videoStream = new VideoStream
            {
                H264 = new VideoStream.Types.H264CodecSettings
                {
                    Profile = "high",
                    BitrateBps = 3000000,
                    FrameRate = 30,
                    HeightPixels = 720,
                    WidthPixels = 1280
                }
            };

            ElementaryStream elementaryStreamVideo = new ElementaryStream
            {
                Key = "es_video",
                VideoStream = videoStream
            };

            AudioStream audioStream = new AudioStream
            {
                Codec = "aac",
                ChannelCount = 2,
                BitrateBps = 160000
            };

            ElementaryStream elementaryStreamAudio = new ElementaryStream
            {
                Key = "es_audio",
                AudioStream = audioStream
            };

            // Encryptions
            Encryption widevineEncryption = new Encryption
            {
                Id = "widevine-cenc",
                SecretManagerKeySource = new Encryption.Types.SecretManagerSource
                {
                    SecretVersion = secretVersion
                },
                DrmSystems = new Encryption.Types.DrmSystems
                {
                    Widevine = new Encryption.Types.Widevine()
                },
                MpegCenc = new Encryption.Types.MpegCommonEncryption()
                {
                    Scheme = "cenc"
                }
            };

            Encryption playreadyEncryption = new Encryption
            {
                Id = "playready-cenc",
                SecretManagerKeySource = new Encryption.Types.SecretManagerSource
                {
                    SecretVersion = secretVersion
                },
                DrmSystems = new Encryption.Types.DrmSystems
                {
                    Playready = new Encryption.Types.Playready()
                },
                MpegCenc = new Encryption.Types.MpegCommonEncryption()
                {
                    Scheme = "cenc"
                }
            };

            Encryption fairplayEncryption = new Encryption
            {
                Id = "fairplay-cbcs",
                SecretManagerKeySource = new Encryption.Types.SecretManagerSource
                {
                    SecretVersion = secretVersion
                },
                DrmSystems = new Encryption.Types.DrmSystems
                {
                    Fairplay = new Encryption.Types.Fairplay()
                },
                MpegCenc = new Encryption.Types.MpegCommonEncryption()
                {
                    Scheme = "cbcs"
                }
            };

            // Mux streams
            MuxStream widevineFmp4Video = new MuxStream
            {
                Key = widevineMuxStreams[0],
                Container = "fmp4",
                ElementaryStreams = { "es_video" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "widevine-cenc"
            };

            MuxStream widevineFmp4Audio = new MuxStream
            {
                Key = widevineMuxStreams[1],
                Container = "fmp4",
                ElementaryStreams = { "es_audio" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "widevine-cenc"
            };

            MuxStream playreadyFmp4Video = new MuxStream
            {
                Key = playreadyMuxStreams[0],
                Container = "fmp4",
                ElementaryStreams = { "es_video" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "playready-cenc"
            };

            MuxStream playreadyFmp4Audio = new MuxStream
            {
                Key = playreadyMuxStreams[1],
                Container = "fmp4",
                ElementaryStreams = { "es_audio" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "playready-cenc"
            };

            MuxStream fairplayFmp4Video = new MuxStream
            {
                Key = fairplayMuxStreams[0],
                Container = "fmp4",
                ElementaryStreams = { "es_video" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "fairplay-cbcs"
            };

            MuxStream fairplayFmp4Audio = new MuxStream
            {
                Key = fairplayMuxStreams[1],
                Container = "fmp4",
                ElementaryStreams = { "es_audio" },
                SegmentSettings = new SegmentSettings
                {
                    SegmentDuration = new Google.Protobuf.WellKnownTypes.Duration
                    {
                        Seconds = 2
                    }
                },
                EncryptionId = "fairplay-cbcs"
            };

            CreateChannelRequest request = new CreateChannelRequest
            {
                ParentAsLocationName = LocationName.FromProjectLocation(ProjectId, LocationId),
                ChannelId = channelId,
                Channel = new Channel
                {
                    InputAttachments = { inputAttachment },
                    Output = new Channel.Types.Output
                    {
                        Uri = outputUri
                    },
                    Encryptions = { widevineEncryption, playreadyEncryption, fairplayEncryption },
                    ElementaryStreams = { elementaryStreamVideo, elementaryStreamAudio },
                    MuxStreams = { widevineFmp4Video, widevineFmp4Audio, playreadyFmp4Video, playreadyFmp4Audio, fairplayFmp4Video, fairplayFmp4Audio },
                    Manifests = {
                        new Manifest {
                            FileName = "manifest_widevine.mpd",
                            Type = Manifest.Types.ManifestType.Dash,
                            MuxStreams = { widevineMuxStreams },
                            MaxSegmentCount = 5
                        },
                        new Manifest {
                            FileName = "manifest_playready.mpd",
                            Type = Manifest.Types.ManifestType.Dash,
                            MuxStreams = { playreadyMuxStreams },
                            MaxSegmentCount = 5
                        },
                        new Manifest {
                            FileName = "manifest_fairplay.m3u8",
                            Type = Manifest.Types.ManifestType.Hls,
                            MuxStreams = { fairplayMuxStreams },
                            MaxSegmentCount = 5
                        }
                    }
                }
            };

            // Make the request.
            Operation<Channel, OperationMetadata> response = await Client.CreateChannelAsync(request);

            // Poll until the returned long-running operation is complete.
            Operation<Channel, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

            // Retrieve the operation result.
            return completedResponse.Result;
        }
        public Channel GetChannel(string channelId)
        {
            GetChannelRequest request = new GetChannelRequest
            {
                ChannelName = ChannelName.FromProjectLocationChannel(ProjectId, LocationId, channelId)
            };

            // Make the request.
            Channel response = Client.GetChannel(request);

            return response;
        }
        public async Task StartChannelAsync(string channelId)
        {
            StartChannelRequest request = new StartChannelRequest
            {
                ChannelName = ChannelName.FromProjectLocationChannel(ProjectId, LocationId, channelId)
            };

            // Make the request.
            Operation<ChannelOperationResponse, OperationMetadata> response = await Client.StartChannelAsync(request);

            // Poll until the returned long-running operation is complete.
            await response.PollUntilCompletedAsync();
        }
        public async Task StopChannelAsync(string channelId)
        {
            StopChannelRequest request = new StopChannelRequest
            {
                ChannelName = ChannelName.FromProjectLocationChannel(ProjectId, LocationId, channelId)
            };

            // Make the request.
            Operation<ChannelOperationResponse, OperationMetadata> response = await Client.StopChannelAsync(request);

            // Poll until the returned long-running operation is complete.
            await response.PollUntilCompletedAsync();
        }
        public async Task DeleteChannelAsync(string channelId)
        {
            DeleteChannelRequest request = new DeleteChannelRequest
            {
                ChannelName = ChannelName.FromProjectLocationChannel(ProjectId, LocationId, channelId)
            };

            // Make the request.
            Operation<Empty, OperationMetadata> response = await Client.DeleteChannelAsync(request);

            // Poll until the returned long-running operation is complete.
            await response.PollUntilCompletedAsync();
        }
    }
}