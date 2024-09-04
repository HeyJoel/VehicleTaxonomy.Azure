param location string
param project string
param uniqueSuffix string

@description('Currently only "dev" environment is supported.')
@allowed(['dev'])
param environmentType string

param tags { *: string }

var configMap = {
  dev: {
    sku: {
      name: 'Standard_LRS'
    }
    retentionPeriodInDays: 7
  }
}
var config = configMap[environmentType]

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  #disable-next-line BCP334
  name: toLower(take(replace('st${project}imp${environmentType}${uniqueSuffix}', '-', ''), 24))
  location: location
  tags: union(tags, {
    Purpose: 'Import files'
  })
  kind: 'StorageV2'
  sku: {
    name: config.sku.name
  }

  resource blobServices 'blobServices' = {
    name: 'default'

    resource taxonomyImportContainer 'containers' = {
      name: 'taxonomy-import'
    }
  }

  // Import CSV files should only be needed during processing, so lets delete them after 7 days
  resource managementPolicies 'managementPolicies' = {
    name: 'default'
    properties: {
      policy: {
        rules: [
          {
            name: 'cleanup-import-files'
            type: 'Lifecycle'
            enabled: true
            definition: {
              actions: {
                baseBlob: {
                  delete: {
                    daysAfterCreationGreaterThan: config.retentionPeriodInDays
                  }
                }
              }
              filters: {
                blobTypes: ['blockBlob']
              }
            }
          }
        ]
      }
    }
  }
}

output storageAccountName string = storageAccount.name
