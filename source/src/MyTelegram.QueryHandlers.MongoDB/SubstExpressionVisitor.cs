namespace MyTelegram.QueryHandlers.MongoDB;

internal class SubstExpressionVisitor : ExpressionVisitor
{
    public Dictionary<Expression, Expression> Subst = new();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Subst.GetValueOrDefault(node, node);
    }
}
