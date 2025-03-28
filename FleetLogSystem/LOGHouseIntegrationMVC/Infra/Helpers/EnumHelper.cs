using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;

namespace LOGHouseSystem
{
    public class EnumStruct
    {
        public string Option { get; set; }

        public int Index { get; set; }

        public string Description { get; set; }
    }

    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return null;

            var option = enumValue.ToString();
            var field = typeof(T).GetField(option);

            if (field == null) return null;

            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>().First();

            return attribute.Description;
        }

        public static IList<string> GetDescriptions<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return null;

            var descriptions = new List<string>();

            var members = typeof(T).GetMembers()
                .Where(member => member.CustomAttributes.Where(attr => attr.AttributeType == typeof(DescriptionAttribute)).Any());

            var attributes = members
                .SelectMany(member => member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>())
                .ToList();

            return attributes.Select(it => it.Description).OrderBy(it => it).ToList();
        }

        public static IList<EnumStruct> GetFullDescriptions<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return null;

            var descriptions = new List<EnumStruct>();

            var members = typeof(T).GetMembers()
                .Where(member => member.CustomAttributes.Where(attr => attr.AttributeType == typeof(DescriptionAttribute)).Any());

            foreach (var member in members)
            {
                var attribute = member
                    .GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .Cast<DescriptionAttribute>()
                    .ToList()
                    .First();

                var values = (T[])Enum.GetValues(typeof(T));
                var index = values.First(it => it.ToString() == member.Name);

                descriptions.Add(new EnumStruct()
                {
                    Index = index.GetHashCode(),
                    Option = member.Name,
                    Description = attribute.Description,
                });
            }

            return descriptions.OrderBy(it => it.Description).ToList();
        }
    }

    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        public static IEnumerable<Dictionary<int, string>> GetProperties()
        {
            var list = new List<Dictionary<int, string>>();

            foreach (int value in Enum.GetValues(typeof(T)))
            {
                var displayName = GetDisplayValue(Parse($"{value}"));
                list.Add(new Dictionary<int, string> { { value, displayName } });
            }

            return list;
        }
    }
}
