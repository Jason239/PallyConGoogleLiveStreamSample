#include <string>
#include <msclr/marshal_cppstd.h>
#include <iostream>

#include "PallyConKmsClientWrapper.h"

namespace PallyCon {
	PallyConKmsClientWrapper::PallyConKmsClientWrapper(String^ strKmsURL, String^ strEncToken)
		: _kmsClient(new PallyConKmsClient(msclr::interop::marshal_as<std::string>(strKmsURL)
			, msclr::interop::marshal_as<std::string>(strEncToken)))
	{
	}

	PallyConKmsClientWrapper::~PallyConKmsClientWrapper()
	{
		delete _kmsClient;
		_kmsClient = nullptr;
	}

	bool PallyConKmsClientWrapper::getPackagingInfoFromKmsServer(String^ content_id
		, String^% key_id, String^% key, String^% iv, String^% hls_key_uri, String^% widevine_pssh, String^% playready_pssh)
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
}