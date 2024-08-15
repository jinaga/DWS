namespace DWS.Model;

[FactType("DWS.Supplier")]
public record Supplier(User creator, Guid supplierGuid)
{
    public Relation<Client> Clients => Relation.Define(facts =>
        from client in facts.OfType<Client>()
        where client.supplier == this && !client.IsDeleted
        select client
    );
}

[FactType("DWS.Administrator")]
public record Administrator(Supplier supplier, User user, DateTime createdDate);

[FactType("DWS.Administator.Revoke")]
public record AdministratorRevoke(Administrator administrator);

[FactType("DWS.Dispatcher")]
public record Dispatcher(Supplier supplier, User user, DateTime createdDate);

[FactType("DWS.Dispatcher.Revoke")]
public record DispatcherRevoke(Dispatcher dispatcher);

[FactType("DWS.Worker")]
public record Worker(Supplier supplier, User user, DateTime createdDate);

[FactType("DWS.Worker.Revoke")]
public record WorkerRevoke(Worker worker);

[FactType("DWS.User.Name")]
public record UserName(User user, string value, UserName[] prior);

[FactType("DWS.Client")]
public record Client(Supplier supplier, Guid clientGuid)
{
    public Relation<ClientName> Names => Relation.Define(facts =>
      from name in facts.OfType<ClientName>()
      where name.client == this &&
        !facts.OfType<ClientName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<ClientDelete>(delete => delete.client == this &&
        !facts.Any<ClientRestore>(restore => restore.clientDelete == delete)
      )
    );
}

[FactType("DWS.Client.Delete")]
public record ClientDelete(Client client, DateTime deletedDate);

[FactType("DWS.Client.Restore")]
public record ClientRestore(ClientDelete clientDelete);

[FactType("DWS.Client.Name")]
public record ClientName(Client client, string value, ClientName[] prior);

[FactType("DWS.Yard")]
public record Yard(Client client, Guid yardGuid)
{
    public Relation<YardName> Names => Relation.Define(facts =>
      from yardName in facts.OfType<YardName>()
      where yardName.yard == this &&
        !facts.Any<YardName>(next => next.prior.Contains(yardName))
      select yardName
    );

    public Relation<YardAddress> Addresses => Relation.Define(facts =>
      from yardAddress in facts.OfType<YardAddress>()
      where yardAddress.yard == this &&
        !facts.Any<YardAddress>(next => next.prior.Contains(yardAddress))
      select yardAddress
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<YardDelete>(delete => delete.yard == this &&
        !facts.Any<YardRestore>(restore => restore.yardDelete == delete)
      )
    );
}

[FactType("DWS.Yard.Delete")]
public record YardDelete(Yard yard, DateTime deletedDate);

[FactType("DWS.Yard.Restore")]
public record YardRestore(YardDelete yardDelete);

[FactType("DWS.Yard.Name")]
public record YardName(Yard yard, string value, YardName[] prior);

[FactType("DWS.Yard.Address")]
public record YardAddress(Yard yard, string street, string number, string postalCode, string place, string country, YardAddress[] prior);

[FactType("DWS.Tool")]
public record Tool(Supplier supplier, Guid toolGuid)
{
    public Relation<ToolName> Names => Relation.Define(facts =>
      from name in facts.OfType<ToolName>()
      where name.tool == this &&
        !facts.OfType<ToolName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<ToolDelete>(delete => delete.tool == this &&
        !facts.Any<ToolRestore>(restore => restore.toolDelete == delete)
      )
    );
}

[FactType("DWS.Tool.Delete")]
public record ToolDelete(Tool tool, DateTime deletedDate);

[FactType("DWS.Tool.Restore")]
public record ToolRestore(ToolDelete toolDelete);

[FactType("DWS.Tool.Name")]
public record ToolName(Tool tool, string value, ToolName[] prior);

[FactType("DWS.TypeOfWork")]
public record TypeOfWork(Supplier supplier, Guid typeOfWorkGuid)
{
    public Relation<TypeOfWorkName> Names => Relation.Define(facts =>
      from name in facts.OfType<TypeOfWorkName>()
      where name.typeOfWork == this &&
        !facts.OfType<TypeOfWorkName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Relation<TypeOfWorkIcon> Icons => Relation.Define(facts =>
      from icon in facts.OfType<TypeOfWorkIcon>()
      where icon.typeOfWork == this &&
        !facts.OfType<TypeOfWorkIcon>().Any(next => next.prior.Contains(icon))
      select icon
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<TypeOfWorkDelete>(delete => delete.typeOfWork == this &&
        !facts.Any<TypeOfWorkRestore>(restore => restore.typeOfWorkDelete == delete)
      )
    );
}

[FactType("DWS.TypeOfWork.Delete")]
public record TypeOfWorkDelete(TypeOfWork typeOfWork, DateTime deletedDate);

[FactType("DWS.TypeOfWork.Restore")]
public record TypeOfWorkRestore(TypeOfWorkDelete typeOfWorkDelete);

[FactType("DWS.TypeOfWork.Name")]
public record TypeOfWorkName(TypeOfWork typeOfWork, string value, TypeOfWorkName[] prior);

[FactType("DWS.TypeOfWork.Icon")]
public record TypeOfWorkIcon(TypeOfWork typeOfWork, string hash, TypeOfWorkIcon[] prior);

[FactType("DWS.TypeOfLeave")]
public record TypeOfLeave(Supplier supplier, Guid typeOfLeaveGuid)
{
    public Relation<TypeOfLeaveName> Names => Relation.Define(facts =>
      from name in facts.OfType<TypeOfLeaveName>()
      where name.typeOfLeave == this &&
        !facts.OfType<TypeOfLeaveName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Relation<TypeOfLeaveIcon> Icons => Relation.Define(facts =>
      from icon in facts.OfType<TypeOfLeaveIcon>()
      where icon.typeOfLeave == this &&
        !facts.OfType<TypeOfLeaveIcon>().Any(next => next.prior.Contains(icon))
      select icon
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<TypeOfLeaveDelete>(delete => delete.typeOfLeave == this &&
        !facts.Any<TypeOfLeaveRestore>(restore => restore.typeOfLeaveDelete == delete)
      )
    );
}

[FactType("DWS.TypeOfLeave.Delete")]
public record TypeOfLeaveDelete(TypeOfLeave typeOfLeave, DateTime deletedDate);

[FactType("DWS.TypeOfLeave.Restore")]
public record TypeOfLeaveRestore(TypeOfLeaveDelete typeOfLeaveDelete);

[FactType("DWS.TypeOfLeave.Name")]
public record TypeOfLeaveName(TypeOfLeave typeOfLeave, string value, TypeOfLeaveName[] prior);

[FactType("DWS.TypeOfLeave.Icon")]
public record TypeOfLeaveIcon(TypeOfLeave typeOfLeave, string hash, TypeOfLeaveIcon[] prior);

[FactType("DWS.Task")]
public record Task(Yard yard, Guid taskGuid)
{
    public Relation<TaskClientName> ClientNames => Relation.Define(facts =>
      from name in facts.OfType<TaskClientName>()
      where name.task == this &&
        !facts.OfType<TaskClientName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Relation<TaskYardName> YardNames => Relation.Define(facts =>
      from name in facts.OfType<TaskYardName>()
      where name.task == this &&
        !facts.OfType<TaskYardName>().Any(next => next.prior.Contains(name))
      select name
    );

    public Relation<TaskYardAddress> YardAddresses => Relation.Define(facts =>
      from address in facts.OfType<TaskYardAddress>()
      where address.task == this &&
        !facts.OfType<TaskYardAddress>().Any(next => next.prior.Contains(address))
      select address
    );

    public Relation<TaskToolLookup> ToolLookups => Relation.Define(facts =>
      from toolLookup in facts.OfType<TaskToolLookup>()
      where toolLookup.task == this
      select toolLookup
    );

    public Relation<TaskToolOnTheFly> ToolOnTheFlys => Relation.Define(facts =>
      from toolOnTheFly in facts.OfType<TaskToolOnTheFly>()
      where toolOnTheFly.task == this
      select toolOnTheFly
    );

    public Relation<TaskWorker> Workers => Relation.Define(facts =>
      from worker in facts.OfType<TaskWorker>()
      where worker.task == this
      select worker
    );

    public Condition IsDeleted => Condition.Define(facts =>
      facts.Any<TaskDelete>(delete => delete.task == this &&
        !facts.Any<TaskRestore>(restore => restore.taskDelete == delete)
      )
    );
}

[FactType("DWS.Task.Delete")]
public record TaskDelete(Task task, DateTime deletedDate);

[FactType("DWS.Task.Restore")]
public record TaskRestore(TaskDelete taskDelete);

[FactType("DWS.Task.ClientName")]
public record TaskClientName(Task task, string value, TaskClientName[] prior);

[FactType("DWS.Task.YardName")]
public record TaskYardName(Task task, string value, TaskYardName[] prior);

[FactType("DWS.Task.YardAddress")]
public record TaskYardAddress(Task task, string street, string number, string postalCode, string place, string country, TaskYardAddress[] prior);

[FactType("DWS.Task.Tool.Lookup")]
public record TaskToolLookup(Task task, Tool tool, DateTime createdDate);

[FactType("DWS.Task.Tool.Lookup.Delete")]
public record TaskToolLookupDelete(TaskToolLookup taskToolLookup);

[FactType("DWS.Task.Tool.OnTheFly")]
public record TaskToolOnTheFly(Task task, string name, DateTime createdDate);

[FactType("DWS.Task.Tool.OnTheFly.Delete")]
public record TaskToolOnTheFlyDelete(TaskToolOnTheFly taskToolOnTheFly);

[FactType("DWS.Task.Worker")]
public record TaskWorker(Task task, Worker worker, TaskWorker[] prior);

