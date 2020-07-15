# Simple Calculator Prototype

A simpler duties and taxes calculator prototype.

## What works

- Forward and reverse calculations, including grey zone detection and handling
- Weight based, rate based and fixed rate calculations.
- Minimum payable and minimum collectible constraints.

## Current gaps

Additional testing potentially needed on grey zones when minimum payable and minimum collectible are involved.

## Endpoints

- https://localhost:5001/api/Calculator/forward
- https://localhost:5001/api/Calculator/reverse

Sample Contract:

```json
{
  "declarationCountry": "IE",
  "deliveryPrice": "EUR100",
  "currency": "EUR",
  "orderItems": [
    {
      "quantity": 1,
      "weight": 1,
      "vatRate": 7,
      "dutyRate": 12,
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

