using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PallyCon
{
    class PallyConHelper
    {
        public class SecretPayload
        {
            public IList<Dictionary<string, object>>? encryptionKeys { get; set; }
        }
        public static string Base64ToHex(string strInput)
        {
            try
            {
                var bytes = Convert.FromBase64String(strInput);
                var hex = BitConverter.ToString(bytes);
                return hex.Replace("-", "").ToLower();
            }
            catch (Exception)
            {
                return "-1";
            }
        }
        public static string GetSecretKeyDataFromPallyConKMS(string kms_url, string enc_token, string content_id,
            RepeatedField<string> widevineMuxStreams, RepeatedField<string> playreadyMuxStreams, RepeatedField<string> fairplayMuxStreams)
        {
            string key_id = "", key = "", iv = "", hls_key_uri = "", widevine_pssh = "", playready_pssh = "";

            // Get the packaging information from PallyCon KMS Server
            PallyConKmsClientWrapper pallyconWrapper = new PallyConKmsClientWrapper(kms_url, enc_token); ;
            pallyconWrapper.getPackagingInfoFromKmsServer(content_id, ref key_id, ref key, ref iv, ref hls_key_uri, ref widevine_pssh, ref playready_pssh);

            // Set the string data to meet Google Cloud Secret Key format
            var widevineDict = new Dictionary<string, object>();
            widevineDict["keyId"] = key_id;
            widevineDict["key"] = Base64ToHex(key);
            widevineDict["keyUri"] = widevine_pssh; // Currently, pssh input does not work in LiveStreamAPI v1.
            widevineDict["matchers"] = new List<object> { new Dictionary<string, object> { ["muxStreams"] = widevineMuxStreams.ToList() } };

            var playreadyDict = new Dictionary<string, object>();
            playreadyDict["keyId"] = key_id;
            playreadyDict["key"] = Base64ToHex(key);
            playreadyDict["keyUri"] = playready_pssh; // Currently, pssh input does not work in LiveStreamAPI v1.
            playreadyDict["matchers"] =  new List<object> { new Dictionary<string, object> { ["muxStreams"] = playreadyMuxStreams.ToList() } };

            var fairplayDict = new Dictionary<string, object>();
            fairplayDict["keyId"] = key_id;
            fairplayDict["key"] = Base64ToHex(key);
            fairplayDict["iv"] = Base64ToHex(iv);
            fairplayDict["keyUri"] = hls_key_uri;
            fairplayDict["matchers"] =  new List<object> { new Dictionary<string, object> { ["muxStreams"] = fairplayMuxStreams.ToList() } };

            var jsonSecretPayload = new SecretPayload
            {
                encryptionKeys = new List<Dictionary<string, object>>() { widevineDict, playreadyDict, fairplayDict }
            };

            return JsonSerializer.Serialize(jsonSecretPayload);
        }
    }
}
