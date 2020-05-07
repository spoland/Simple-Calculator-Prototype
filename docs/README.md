# Simple Calculator Prototype

A simpler duties and taxes calculator prototype.

## What works (mostly 😃)

- Forward and reverse calculations that don't fall into grey zones.
- Weight based, rate based and fixed rate calculations.
- Minimum payable and minimum collectible constraints.

## Current gaps

The calculator has not yet been configured to  handle grey zones, and most likely won't correctly reverse out constraints in all cases.

## Endpoints

- https://localhost:5001/api/Calculator/forward
- https://localhost:5001/api/Calculator/reverse

Sample Contract:

```json
{
  "id": "123",
  "countryIso": "IE",
  "currencyIso": "EUR",
  "orderItems": [
    {
      "quantity": 1,
      "weight": 1,
      "vatRate": 7,
      "dutyRate": 10,
      "price": "EUR100"
    }
  ]
}
```

## Configuration

A sample calculator configuration (untested, just includes all current options):

```json

{
      "Id": "IE",
      "DeminimisBaseCharges": [ "Item" ],
      "Excess":  "EUR10",
      "ChargeConfigurations": [
        {
          "Name": "Duty",
          "CalculationType": "RateBased",
          "BaseCharges": [ "Item" ],
          "DeminimisThreshold": "EUR0",
          "MinimumCollectible": "EUR10"
        },
        {
          "Name": "Vat",
          "CalculationType": "WeightBased",
          "Rate":  10,
          "DeminimisThreshold": "EUR0",
          "MinimumPayable":  "EUR10"
        },
        {
          "Name": "Interest",
          "CalculationType": "RateBased",
          "BaseCharges": [ "Item", "Duty", "Vat" ],
          "Rate": 10,
          "DeminimisThreshold": "EUR0"
        }
      ]
    }

```

