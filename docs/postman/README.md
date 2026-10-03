# Postman collection

Import [NetAspireServer.postman_collection.json](NetAspireServer.postman_collection.json) into Postman to try the local API.

## Run

1. Start the API, preferably through the Aspire AppHost.
2. Import the collection into Postman.
3. Check the collection variable `baseUrl`. It defaults to `https://localhost:5001`; change it if Aspire shows a different API URL. The HTTP launch profile is available at `http://localhost:5000`.
4. Run **Create product** before **Get product by ID**. The create request stores the returned ID in the collection variable `productId`.

The collection includes the status and health endpoints, product listing, creation, lookup by ID, and an invalid-product request that expects a validation error.
