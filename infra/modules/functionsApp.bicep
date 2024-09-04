param location string
param project string
param uniqueSuffix string

@description('Currently only "dev" environment is supported.')
@allowed(['dev'])
param environmentType string

param tags { *: string }
param cosmosDbAccountName string
param cosmosDbDatabaseName string
param importFileStorageAccountName string
param logAnalyticsWorkspaceId string

var configMap = {
  dev: {
    storageAccount: { 
      sku: {
        name: 'Standard_LRS'
      }
    }
  }
}
var config = configMap[environmentType]
var functionAppName = toLower(take('func-${project}-${environmentType}-${uniqueSuffix}', 60))

// TODO: RBAC? KeyVault? For now we can reference these resources as existing and use that to pass in CS etc
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosDbAccountName
}

resource importFileStorageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: importFileStorageAccountName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  #disable-next-line BCP334
  name: toLower(take(replace('st${project}func${environmentType}${uniqueSuffix}', '-', ''), 24))
  location: location
  tags: union(tags, {
    Purpose: 'Function App'
  })
  sku: {
    name: config.storageAccount.sku.name
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

resource appPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: toLower(take('app-${project}-${environmentType}-${uniqueSuffix}', 60))
  location: location
  tags: tags
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '~14'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'BlobStorage:ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${importFileStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${importFileStorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'CosmosDb:ConnectionString'
          value: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: cosmosDbDatabaseName
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: toLower('inc-${project}-${environmentType}')
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output functionBaseUrl string = 'https://${functionApp.properties.defaultHostName}/api' 
output functionAppName string = functionApp.name
