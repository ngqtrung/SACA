using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SortOrderAttribute : Attribute
{
    public double Order { get; }

    public SortOrderAttribute(double order)
    {
        Order = order;
    }
}
