    . Database: if available within the log => this placeholder should include all the cosmosdb databases that are accessed during the method execution.
      every database value should be reported with the following format:
      <code>'{{DatabaseUrl}}'</code>, Collection: '<code>{{Table}}</code>'
      if no databases are within the log => no database placeholder should be included into the table.
    . ServiceBus: if available within the log => this placeholder should include all the service bus accessed during the method execution.
      every service bus should be reported with the following format:
      <code>{{ServiceBusBaseUrl}}</code>
      if no databases are within the log => no ServiceBus placeholder should be included into the table.
    . HttpExternalService: if available within the log => this placeholder should include all the service bus accessed during the method execution.
      every service bus should be reported with the following format:
      <code>{{HttpBaseUrl}}</code>
      if no databases are within the log => no HttpExternalService placeholder should be included into the table.
