# Infra: Bicep

## Getting Started

```
// TODO: name required? must be unique but if excluded I expect it generates one. Not included in my example
az deployment sub create --location=eastus --parameters main.dev.bicepparam
```

## Deleting infra

```
az group delete --name rg-veh-tax-bic-dev
```
