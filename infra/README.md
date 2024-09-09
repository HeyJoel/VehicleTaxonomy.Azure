# Infra: Bicep

[Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview/) is used to deploy the Azure resources required to support the API deployment. While care has been taken to utilise free or low cost Azure SKUs, some cost may be incurred in deploying and running the sample.

## Getting Started

Prerequisits:

- Install the [Azure and Bicep CLI tools](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install)
- You will also need an Azure account and be [logged into the Azure CLI](https://learn.microsoft.com/en-us/cli/azure/authenticate-azure-cli), typically via [`az login`](https://learn.microsoft.com/en-us/cli/azure/authenticate-azure-cli-interactively)

The bicep deployment runs at the subscription level, nesting all resources under a newly created resource group called "rg-veh-tax-bic-dev". While the bicep files allow different environment configurations, only "dev" is supported in this example.

You can use the "what-if" tool to preview infrastructure changes: 

```
az deployment sub what-if --location=eastus2 --parameters main.dev.bicepparam
```

or run the deploying using:
```
az deployment sub create --location=eastus --parameters main.dev.bicepparam
```

## Deleting infra

You can remove all resources by deleting the resource group:

```
az group delete --name rg-veh-tax-bic-dev
```
