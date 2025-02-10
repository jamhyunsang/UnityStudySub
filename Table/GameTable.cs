using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class NonPublicDefaultContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (!property.Writable)
        {
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                var setter = propertyInfo.GetSetMethod(true);
                if (setter != null)
                {
                    property.Writable = true;
                }
            }
        }

        return property;
    }
}

public class TestTable
{
    public int Key { get; private set; }
    public int intValue { get; private set; }
    public float floatValue { get; private set; }
    public string stringValue { get; private set; }
    public long longValue { get; private set; }
    public double doubleValue { get; private set; }
}

public class GameTable
{
    public static Dictionary<TableType, object> Parse(JArray Obj)
    {
        Dictionary<TableType, object> Result = new Dictionary<TableType, object>();
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new NonPublicDefaultContractResolver(),
            Formatting = Formatting.Indented
        };
        for (int Count = 0; Count < Obj.Count; ++Count)
        {
            var CurTableName = Obj[Count]["Key"].Value<string>();
            var CurTableData = Obj[Count]["Value"].Value<string>();
            switch (CurTableName)
            {
                case "TestTable":
                    Result.Add(TableType.TestTable, JsonConvert.DeserializeObject<List<TestTable>>(CurTableData, settings));
                    break;
                default:
                    throw new ArgumentException("Invalid TableType");
            }
        }

        return Result;
    }
}

public enum TableType
{
    TestTable,
    End
}