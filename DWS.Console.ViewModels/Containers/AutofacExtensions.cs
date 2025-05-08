using Autofac.Core.Resolving.Pipeline;
using Autofac.Core.Resolving;
using Autofac.Core;
using Autofac.Builder;

namespace DWS.Console.ViewModels.Containers;

// Adapted from https://stackoverflow.com/a/68601768
public static class AutofacExtensions
{
    public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> TransitiveFactory<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
    {
        return builder.OnPreparing(ForwardFactoryParameters);
    }

    private static void ForwardFactoryParameters(PreparingEventArgs e)
    {
        var ctx = e.Context;
        var operProperty = ctx.GetType().GetProperty("Operation");
        if (operProperty == null)
        {
            throw new InvalidOperationException("The 'Operation' property was not found on the context.");
        }

        var oper = operProperty.GetValue(ctx);
        if (oper == null)
        {
            throw new InvalidOperationException("The 'Operation' property value is null.");
        }

        var requestStackProperty = oper.GetType().GetProperty("InProgressRequests");
        if (requestStackProperty == null)
        {
            throw new InvalidOperationException("The 'InProgressRequests' property was not found on the operation.");
        }

        var requestStack = requestStackProperty.GetValue(oper) as SegmentedStack<ResolveRequestContext>;
        if (requestStack == null)
        {
            throw new InvalidOperationException("The 'InProgressRequests' property value is null or not of the expected type.");
        }

        if (requestStack.Count == 1)
        {
            // Nothing to do; we are on the first level of the call stack.
            return;
        }

        var entryRequest = requestStack.Last();
        e.Parameters = entryRequest.Parameters;
    }
}
