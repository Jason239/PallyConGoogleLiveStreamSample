#pragma once

#include <string>
#include <memory>
#include <map>
#include <stdexcept>


typedef enum {
	NONE = 0,
	CMAF = (1 << 0),		// 0000 0001 // 0x01
	DASH = (1 << 1),		// 0000 0010 // 0x02
	HLS = (1 << 2),			// 0000 0100 // 0x04
	HLS_NCG = (1 << 3),		// 0000 1000 // 0x08
	NCG = (1 << 4)			// 0001 0000 // 0x10
} PackType;

inline PackType operator&(PackType type_a, PackType type_b) {
	return static_cast<PackType>(static_cast<int>(type_a) & static_cast<int>(type_b));
}

inline PackType operator|(PackType type_a, PackType type_b) {
	return static_cast<PackType>(static_cast<int>(type_a) | static_cast<int>(type_b));
}

inline PackType operator^(PackType type_a, PackType type_b) {
	return static_cast<PackType>(static_cast<int>(type_a) ^ static_cast<int>(type_b));
}

inline PackType& operator&=(PackType& type_a, PackType type_b) {
	return (PackType&)((int&)(type_a) &= (int)(type_b));
}

inline PackType& operator|=(PackType& type_a, PackType type_b) {
	return (PackType&)((int&)(type_a) |= (int)(type_b));
}

inline PackType& operator^=(PackType& type_a, PackType type_b) {
	return (PackType&)((int&)(type_a) ^= (int)(type_b));
}

struct ArrayDeleter
{
	template<typename T>
	void operator()(T *p)
	{
		delete[] p;
	}
};

struct MultiTracksInfo
{
	std::string type;
	std::string key;
	std::string keyId;
	std::string iv;
	std::string hlsKeyUri;
	std::string pssh_widevine;
	std::string pssh_playready;
};

struct ContentPackagingInfo
{
	int keyCount;
	std::shared_ptr<MultiTracksInfo> tracks;
	std::string contentId;
	std::string key;
	std::string keyId;
	std::string iv;
	std::string hlsKeyUri;
	std::string pssh_widevine;
	std::string pssh_playready;
	std::string clearkey_key;
	std::string clearkey_keyId;
	std::string ncg_cek;
};

class PallyConKmsClient
{
private:
	std::string _kmsUrl;
	std::string _encToken;

	int _lastRequestStatus;
	std::string _lastRequestRowData;
	std::string _lastResponseRowData;
	
	std::map<std::string, std::string> m_keysMap;
	std::map<std::string, std::string> m_requestParamMap;

	std::string getRequestData(std::string contentId, std::string trackInfo, PackType packType);
	ContentPackagingInfo parseResponse(const std::string& responseBody);
	ContentPackagingInfo parseNcgResponse(const std::string& responseBody);
public:
	PallyConKmsClient(std::string strKmsURL, std::string strEncToken);
	~PallyConKmsClient();

	int getLastRequestStatus() const {
		return _lastRequestStatus;
	}

	std::string getLastRequestRowData() const {
		return _lastRequestRowData;
	}

	std::string getLastResponseRowData() const {
		return _lastResponseRowData;
	}
	
	/**
	* Add the request parameters.
	*
	* @param key					Name of the parameter
	* @param value					Value of the parameter
	*/
	void addRequestParam(const std::string& key, const std::string& value);

	/**
	* Receive packaging information from the PallyCon KMS server.
	*
	* @param cid					Content id
	* @param trackInfo				Track information separated by delimiter('|') for multi-key packaging. (e.g. SD|HD|AUDIO)
	*								For single-key packaging, it should be an empty.
	* @param packType				Packaging type. (e.g. Dash, Hls, etc)
	*/
	ContentPackagingInfo getContentPackagingInfoFromKmsServer(const std::string cid, const std::string trackInfo, PackType packType);
};

