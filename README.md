---------------------------------------
# Google Cloud Live Stream API & PallyCon DRM Integration Sample
This sample shows how to integrate PallyCon Multi DRM with Google Cloud Live Stream API v1 using [API Client Libraries](https://cloud.google.com/livestream/docs/reference/libraries). Since this sample focused on DRM integration, only a simple scenario of applying Widevine, PlayReady, and FairPlay DRM to a live stream in fmp4 format is used, see the [references link](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.Video.LiveStream.V1/latest) for more information about Live Stream API v1 features.



---------------------------------------
## Prerequisites
- A Windows 10/11 PC
- Visual Studio 2022 (Windows 10/11)
- .NET 6.0 SDK : https://dotnet.microsoft.com/download
- Google Cloud account : https://cloud.google.com/
  - Create a project : https://cloud.google.com/resource-manager/docs/creating-managing-projects
  - Create a bucket : https://cloud.google.com/storage/docs/creating-buckets
- Google Application Default Credentials(ADC) : https://cloud.google.com/docs/authentication/provide-credentials-adc

- KMS token used for CPIX API communication with PallyCon KMS. This is an API authentication token that is generated when you sign up PallyCon service, and can be found on the PallyCon Console site.
- Encoder to generate the input stream that the API processes.

  - In this sample, [ffmpeg](https://ffmpeg.org/download.html) is used.

  
---------------------------------------
## How to launch the project and test
1. Clone or download this sample repository.
2. Open the root /PallyConGoogleLiveStreamSample.sln and select the active project to launch in Visual Studio.
3. Make sure you have your Google Cloud project, bucket information and PallyCon KMS related information.
4. Set the values of the variables at the top of the main method.
5. Run the project.
6. Copy the  `<RTMP input endpoint uri>` that is printed to the console.
7. When the streamingState is `AwaitingInput` and you see the output saying `the channel is ready`, send a live stream to the input endpoint as shown below.

   ```shell
   $ ffmpeg -re -f lavfi -i "testsrc=size=1280x720 [out0]; sine=frequency=500 [out1]" \
     -acodec aac -vcodec h264 -f flv <RTMP input endpoint uri>
   ```
8. Check out your packaging results on the Buckets page in Google Cloud console.

   
---------------------------------------
## PallyConKMSClientWrapper
C++/CLI project for wrap a C++ library(*PallyConKmsClient_MD.lib*) to communicate with PallyCon KMS server.
The _getPackagingInfoFromKmsServer_ function allows you to obtain packaging information from the KMS server.



```c#
bool PallyConKmsClientWrapper::getPackagingInfoFromKmsServer(String^ content_id, String^% key_id, String^% key, String^% iv, String^% hls_key_uri, String^% widevine_pssh, String^% playready_pssh)
{
	try
	{
		ContentPackagingInfo packInfos = _kmsClient->getContentPackagingInfoFromKmsServer(
			msclr::interop::marshal_as<std::string>(content_id), "", PackType::DASH | PackType::HLS);
		key_id = gcnew String(packInfos.keyId.c_str());
		key = gcnew String(packInfos.key.c_str());
		iv = gcnew String(packInfos.iv.c_str());
		hls_key_uri = gcnew String(packInfos.hlsKeyUri.c_str());
		widevine_pssh = gcnew String(packInfos.pssh_widevine.c_str());
		playready_pssh = gcnew String(packInfos.pssh_playready.c_str());
	}
	catch (std::exception& e)
	{
		std::cout << e.what();
	}

	return true;
}
```



---------------------------------------

## References
- https://pallycon.com/docs/en/multidrm/
- https://pallycon.com/docs/en/multidrm/packaging/cpix-api/
- https://cloud.google.com/livestream/docs/reference/libraries
- https://cloud.google.com/secret-manager/docs/reference/libraries#client-libraries-install-csharp
- https://cloud.google.com/livestream/docs/reference/drm#string
---------------------------------------
