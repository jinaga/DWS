using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWS.Model;


[FactType("DWS.CRMClient")]
public record CRMClient(Supplier supplier, string crmId);

[FactType("DWS.CRMYard")]
public record CRMYard(CRMClient crmClient, string yardId);

[FactType("DWS.CRMYard.Address")]
public record CRMYardAddress(CRMYard crmYard, string street, string number, string city, string postalCode, string country, CRMYardAddress[] prior);

[FactType("DWS.Client.CRM")]
public record ClientCRM(Client client, CRMClient crmClient, DateTime createdDate);

[FactType("DWS.Yard.CRM")]
public record YardCRM(Yard yard, CRMYard crmYard, DateTime createdDate);

[FactType("DWS.Yard.CRM.Address")]
public record YardCRMAddress(YardAddress yardAddress, CRMYardAddress crmYardAddress, DateTime createdDate);

