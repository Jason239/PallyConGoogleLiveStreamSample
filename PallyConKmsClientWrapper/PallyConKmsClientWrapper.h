#pragma once

#include "include/PallyConKmsClient.h"

using namespace System;

extern "C" {FILE __iob_func[3] = { stdin ,stdout,stderr }; }

namespace PallyCon {
	public ref class PallyConKmsClientWrapper
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	private:
		PallyConKmsClient* _kmsClient;

	public:
		PallyConKmsClientWrapper(String^ strKmsURL, String^ strEncToken);
		virtual ~PallyConKmsClientWrapper();
		bool getPackagingInfoFromKmsServer(String^ content_id, String^% key_id, String^% key, String^% iv
			, String^% hls_key_uri, String^% widevine_pssh, String^% playready_pssh);
	};
}