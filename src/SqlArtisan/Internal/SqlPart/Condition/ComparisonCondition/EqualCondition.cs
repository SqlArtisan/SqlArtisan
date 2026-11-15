namespace SqlArtisan.Internal;

internal sealed class EqualCondition(
    SqlExpression leftSide,
    SqlExpression rightSide) : EqualityCondition
{
    internal override SqlExpression LeftSide => leftSide;

    internal override SqlExpression RightSide => rightSide;

    // Format renders the condition, where a converter's NULL never matches (#810);
    // FormatAsAssignment renders SET, where it is the value assigned.
    internal override void Format(SqlBuildingBuffer buffer)
    {
        if (RightSide is ConvertedNull converted)
        {
            throw converted.Compared(Operators.Equal);
        }

        FormatAsAssignment(buffer, qualified: true);
    }

    // An unqualified target column has its why at DbColumn.FormatUnqualified; the
    // right side keeps its normal qualification.
    internal void FormatAsAssignment(SqlBuildingBuffer buffer, bool qualified = false)
    {
        if (!qualified && LeftSide is DbColumn column)
        {
            column.FormatUnqualified(buffer);
        }
        else
        {
            LeftSide.Format(buffer);
        }

        buffer.EncloseInSpaces(Operators.Equal).Append(RightSide);
    }

    // By the target's alias, not the column's owner: the SET guard matches a column by its
    // table's name alone, so another handle's alias would name another relation.
    internal void FormatAsAssignment(SqlBuildingBuffer buffer, DbTableBase target)
    {
        buffer.EncloseInAliasQuotes(target.CorrelationName);
        buffer.Append('.');
        ((DbColumn)LeftSide).FormatUnqualified(buffer);
        buffer.EncloseInSpaces(Operators.Equal).Append(RightSide);
    }
}
