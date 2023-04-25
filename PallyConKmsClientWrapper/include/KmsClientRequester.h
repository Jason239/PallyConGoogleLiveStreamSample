#pragma once

#include "PallyConKmsClient.h"
#include "IHttpRequester.h"

class KmsClientRequester
{
public:
	KmsClientRequester(const std::string& serverURL, const std::string& encToken, NCGCOREHELPER_NS::IHttpRequesterPtr requester)
	{
		_serverURL = serverURL;
		_serverURL.append(encToken); // kms url format : serverurl/enctoken
		_requester = requester;
	}

	void setRequestData(const std::string& requestData) {
		_requestData = requestData;
	}
	
	std::string request(const std::map<std::string, std::string> requestParams);

private:

	std::string _serverURL;
	std::string _requestData;
	NCGCOREHELPER_NS::IHttpRequesterPtr _requester;

private:
	std::string makeRequestParamFromMap(const std::map<std::string, std::string> map);
};

