# List Licenses

List information about installed licenses

## Usage

```
D:\dotnet\dotnet.exe msbuild apps\list_licenses\launch.xml
```

## Example output

```
---------------------------------------------
licenseId = 00000000-0000-0000-0000-000000000000
keyId = 00000000-0000-0000-0000-000000000000
licenseFlags = 0x00000012
        HAS_SUBSCRIPTION_ID
        PERSIST
rootKeyId = 00000000-0000-0000-0000-000000000000
subscriptionId = 00000000-0000-0000-0000-000000000000
discId = 00000000-0000-0000-0000-000000000000
issueDate = 3400303000
beginDate = 0
expirationDate = 0
licenseXml = <License xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" LicenseProtocolVersion="5" xmlns="http://schemas.microsoft.com/xboxlive/security/clas/LicResp/v1"><SignedLicense>...</SignedLicense><Signature DigestAlgorithm="SHA256" SignaturePadding="PSS"><SigningKeyHash>...</SigningKeyHash><SignatureValue>...</SignatureValue></Signature><LicenseResponseMetadata><LicenseId>00000000-0000-0000-0000-000000000000</LicenseId><KeyId>00000000-0000-0000-0000-000000000000</KeyId></LicenseResponseMetadata></License>
policy = {"enforceConcurrency":true,"entitlementId":"00000000000000000000000000000000","entitlementSatisfaction":"SubscriptionDevice","expiresAt":"2024-06-24T15:40:11.1735000+00:00","isOffline":true,"leaseEnforcement":"EnforceConcurrency","leaseUri":"https://licensing.md.mp.microsoft.com/v7.0/licenses/...","legacyProductId":"00000000-0000-0000-0000-000000000000","keyIds":["00000000-0000-0000-0000-000000000000"],"kind":"Content","packages":[{"packageIdentifier":"00000000-0000-0000-0000-000000000000","packageType":"Xvc","productAddOns":[],"productId":"9NHDFTCL691C","skuId":"0010"}],"pollAt":"2024-06-12T15:40:11.1735000+00:00","refreshOnStartup":true,"root":"https://licensing.md.mp.microsoft.com/v7.0/licenses/...","subscriptionProductId":"CFQ7TTC0K6L8","version":7}
```