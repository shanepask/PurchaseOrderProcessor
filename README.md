# PurchaseOrderProcessor
## Documentation
For the system design documentation, goto https://github.com/shanepask/PurchaseOrderProcessor/blob/master/documentation/Design%20Document.md

## To run the application
### 1. Build the docker container
```docker-compose -f docker-compose.yml -p purchase-order-processor build --no-cache```
### 2. Run the container
```docker-compose -f docker-compose.yml -p purchase-order-processor up```
### 3. Open browser
Naviagte to ```http://localhost:5000/swagger/index.html``` and run the examples.