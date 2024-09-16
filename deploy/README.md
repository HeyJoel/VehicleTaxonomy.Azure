# Deployment: Azure DevOps Pipelines

[Azure Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/get-started/what-is-azure-pipelines) are used to deploy the application and all associated infrastructure.

## Getting Started

Prerequisits:

- You will need an Azure account.
- You will need an [Azure DevOps](https://azure.microsoft.com/en-us/products/devops/) account. 
- If you've created a new free DevOps account, you'll need to [apply for the free parallel job grant](https://learn.microsoft.com/en-us/azure/devops/pipelines/licensing/concurrent-jobs), which can take a day or two to come through.

Instructions for registering a new pipeline are out of scope for these docs, see [Azure Pipelines docs](https://learn.microsoft.com/en-us/azure/devops/pipelines/create-first-pipeline) for more info.

### Service Connections:

You will need to create two [service connections](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints), each linked to a subscription:

- `VehicleTaxonomyAzure.non-prod`: This is used for the "dev" environment. If we were to use other environments such as "uat" or "staging" these would also come under this service connection/subscription.
- `VehicleTaxonomyAzure.prod`: This is only used for the "prod" environment, allowing us to separate and secure this environment if required.

The pipeline runs the [IaC bicep code](/infra), creating a resource group for the environment and any required infrastructure.

### Environments

You will need two [pipeline environments](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/environments), `dev` and `prod`. It's expected that you set up an [approval check](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/approvals) on the `prod` environment, allowing you to check the results of the bicep `what-if` command before proceeding with deployment, but it's not required.

## Tear down

You can delete the deployed Azure resources by deleting their resource groups, either via the Azure portal or via the Azure CLI:

```ps
az group delete --name rg-veh-tax-bic-dev --no-wait
az group delete --name rg-veh-tax-bic-prod --no-wait
```