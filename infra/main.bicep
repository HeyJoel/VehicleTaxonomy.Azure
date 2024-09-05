targetScope = 'subscription'

@description('The location to deploy the project e.g. "eastus2"')
param location string

@description('The application deployment environment. Currently only "dev" environment is supported.')
@allowed(['dev'])
param environmentType string

var project = 'veh-tax-bic'
var uniqueSuffix = uniqueString(subscription().id, project, environmentType)
var tags = {
  Project: project
  DeploymentSource: 'Bicep'
  Envionment: environmentType
}

resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-${project}-${environmentType}'
  location: location
}

module resources 'resources.bicep' = {
  name: '${deployment().name}-resources'
  scope: resourceGroup
  params: {
    location: location
    project: project
    environmentType: environmentType
    tags: tags
    uniqueSuffix: uniqueSuffix
  }
}

output resourceGroupName string = resourceGroup.name
output functionAppName string = resources.outputs.functionAppName
output functionBaseUrl string = resources.outputs.functionBaseUrl
