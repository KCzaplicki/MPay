# MPay
[![Version](https://img.shields.io/docker/v/krystianczaplicki/mpay/latest?arch=arm64)](https://hub.docker.com/r/krystianczaplicki/mpay)
![Docker Image Size (tag)](https://img.shields.io/docker/image-size/krystianczaplicki/mpay/latest)
![GitHub issues](https://img.shields.io/github/issues/KCzaplicki/mpay)
![GitHub](https://img.shields.io/github/license/kczaplicki/mpay)

MPay is a lightweight and customizable mock service that emulates a real payment gateway. It enables developers to test payment integration without actual financial transactions in a development or testing environment.

![MPay - architecture overview](https://github.com/KCzaplicki/MPay/blob/main/docs/mpay-architecture-overview.png?raw=true)

## Getting started
### Setup
Get docker image from docker hub
```
docker pull krystianczaplicki/mpay
```

Start docker container with mpay image
```
docker run -it -p 5000:80 -e Webhooks__Url=http://YOUR_WEBHOOK_URL krystianczaplicki/mpay
```
Replace `YOUR_WEBHOOK_URL` with the url on which you want to get purchase updates i.e. http://localhost:5001/api/hooks/payment

🎉 **Configratulations!**  Your MPay service is ready on port **5000**. 🚀

<br/>Now you can follow next section to prepare webhook consumer or jump to [workflow](#workflow) section to create your first purchase.<br/><br/>

### Consume webhook requests

On the webhook url provided in [setup](#setup) section you will recieve http requests with purchase updates.

Webhook http requests are **POST** type with following paylaod:

```
{
    "id": "string", // Purchase guid
    "status": number, // Purchase status
    "updatedAt": date // Update date
}
```

Purchase status can be one of these
|Name|Value|Description|
|-|-|-|
|Created|1|Purchase created, waiting for payment|
|Completed|2|Purchase paid by the user|
|Cancelled|3|Purchase cancelled by the user|
|Timeout|4|Purchase has exceeded its lifetime and has been withdrawn by the system|

<br/>You can find more about purchase statuses on [workflow](#workflow) section.

There are more configuration options available, if you want to explore them, go to [configuration](#configuration) section.

## About
MPay is a lightweight and versatile solution designed to simulate payment gateway functionality in a development or testing environment. It provides a reliable and customizable mock service that emulates the behavior and responses of a real payment gateway, allowing developers to test their applications payment integration without making actual financial transactions.

<br/>**Key Features:**

- **Simulates payment gateway behavior:** MPay accurately replicates the essential features and responses of a payment gateway, enabling developers to validate their payment processing logic effectively.

- **Customizable responses:** Developers can configure the mock service to simulate various payment scenarios, such as successful transactions, declined payments, or specific error conditions, ensuring thorough testing of their application payment handling capabilities.

- **Easy integration:** MPay is designed to be easily integrated into existing development environments. It supports popular programming languages and frameworks, providing a seamless experience for developers working on payment-related features.

- **Realistic testing environment:** By utilizing MPay developers can create a realistic and controlled testing environment, reducing the need for reliance on external payment gateways during the development and testing phases.

- **Secure and private:** As a self-contained mock service, MPay ensures that sensitive payment information is not transmitted or stored, providing an added layer of security during development and testing activities.

## Configuration
MPay configuration can be adjusted with `ENVIRONMENT` variables or by providing `-e` parameter with configuration name on docker container run, i.e.

```
docker run -it -p 5000:80 -e Webhooks__Url=http://YOUR_WEBHOOK_URL krystianczaplicki/mpay
```

Set configuration for webhooks url to `http://YOUR_WEBHOOK_URL`.  
  
<br/>MPay has following feature flags:
|Name|Configuration key|Default Value|Description|
|-|-|-|-|
|Webhooks|FeatureFlags__Webhooks|true|Webhooks feature to notify about purchase updates with http requests|
|Purchase timeout|FeatureFlags__PurchaseTimeout|true|Purchase timeout feature to expire purchases after time to life value is exceeded|

<br/>Configuration options for webhooks
|Name|Configuration key|Default Value|Description|
|-|-|-|-|
|Url|Webhooks__Url|https://localhost:5050/hook|Url on which webhook will notify about purchase updates|
|Retry limit|Webhooks__RetryLimit|3|Retry attempts on webhook http request failure|
|Retry intervals|Webhooks_RetryIntervalsInSeconds|[1, 2, 5]|Intervals in seconds between requests when trying to retry webhook http request on failure|

<br/>Configuration options for purchase timeout
|Name|Configuration key|Default Value|Description|
|-|-|-|-|
|Purchase timeout job interval|PurchaseTimeout__<br/>IntervalInSeconds|60|Interval in seconds between between subsequent purchase checks and status changes for expired purchases|
|Purchase creation timeout|PurchaseTimeout__<br/>PurchaseCreationTimeoutInMinutes|15|Time in minutes after which created purchases will be changed to timeout|
|Purchase last payment timeout|Purchasetimeout__<br/>PurchaseLastPaymentTimeoutInMinutes|15|Time in minutes for last payment attempt after which purchases will be changed to timeout|
|Purchase with payment timeout|Purchasetimeout__<br/>PurchaseWithPaymentsTimeoutInMinutes|60|Time in minutes after which purchases with at least 1 payment will be changed to timeout|

<br/>Other configuration options
|Name|Configuration key|Default Value|Description|
|-|-|-|-|
|Allowed hosts|AllowedHosts| * |Hosts allowed to make http requests to MPay, asterisk means that all hosts are allowed|
|Cors origins|CorsOrigins| * |Origins allowed to make http requests to MPay through web browser, asterisk means that all origins are allowed|

<br/>Beside the configuration you can map following directories to keep MPay data locally:
- MPayDatabase.db - SQLite database used by the MPay
- /Logs - Logs directory used by the MPay, contains logs in *.txt format grouped by the date

## Workflow
Purchase workflow should guide you safely through all possible purchase states with the triggers that activates them.
![MPay - purchase workflow](https://github.com/KCzaplicki/MPay/blob/main/docs/mpay-purchase-workflow.png?raw=true)

## Endpoints
Here is the list of you get all the possible options to explore MPay API. 

Have fun playing with it! 🏄

### Swagger
You can explore MPay endpoints with swagger on development mode. Set `Environment` to `Development` to enable it.
```
docker run -it -p 5000:80 -e ASPNETCORE_ENVIRONMENT=Development krystianczaplicki/mpay
```
Swagger is available on route http://localhost:5000/swagger.

### REST Client
MPay supports explore API with [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for Visual Studio Code.

API Requests are in [MPay.rest](https://github.com/KCzaplicki/MPay/blob/main/api/src/MPay.API/MPay.rest) file, open it in Visual Studio Code.

Don't forget to set proper `@url` value before you start making requests.

### API Docs
You can explore MPay API endpoints from manual documentation.

API Endpoints
|Url|Response|Description|
|-|-|-|
|GET /|MPay API|API Footprint for quick check if it's up and ready|
|GET /heathz|Healthy / Unhealthy|Healthcheck endpoint for other services to check API status|
|GET /configuration|{<br/>&emsp;"webhooks": boolean,<br/>&emsp;"purchaseTimeout": boolean<br/>}|Configuration endpoint to get current MPay feature flags configurations|

Purchase Endpoints
|Url|Payload|Response|Description|
|-|-|-|-|
|POST /purchases|{<br/>&emsp;"name": "string",<br/>&emsp;"description": "string",<br/>&emsp;"price": decimal,<br/>&emsp;"currency": "string"<br/>}|Purchase Id (Guid)|Create purchase endpoint, returns purchase Id| 
|GET /purchases/{id}|-|{<br/>&emsp;"id": "string",<br/>&emsp;"name": "string",<br/>&emsp;"description": "string",<br/>&emsp;"createdAt": date,<br/>&emsp;"price": decimal,<br/>&emsp;"currency": "string"<br/>}|Get purchase by Id, if purchase is not in pending status it will return 404 - not found http status|
|POST /purchases/{id}/cancel|-|-|Cancel purchase endpoint, returns 204 no matter if purchase is cancelled or state not changed|

Purchase Payment Endpoints
|Url|Payload|Description|
|-|-|-|
|POST /purchases/{id}/payment|{<br/>&emsp;"cardHolderName": “string,<br/>&emsp;"cardNumber": number,<br/>&emsp;"ccv": number,<br/>&emsp;"cardExpiry": date<br/>}|Purchase payment endpoint, returns 200 - http status OK no matter if payment passed or not|

To mock different purchase payment paths you have to set specific card number in purchase payment payload.
|Path|Card number|Description|
|-|-|-|
|Invalid card|Card number ends with **1**|Purchase stay in pending state, failed payment data noted|
|No funds|Card number ends with **2**|Purchase stay in pending state, failed payment data noted|
|Timeout|Card number ends with **3**|Purchase stay in pending state, failed payment data noted|
|Payment passed|Card number ends with anything higher than 3, i.e. **4** or **5**|Payment completed successfully, webhook request sent|

<br/>Don't forget that payloads will be validated. You can find validation in [Validators](https://github.com/KCzaplicki/MPay/tree/main/api/src/MPay.Core/Validators) directory.