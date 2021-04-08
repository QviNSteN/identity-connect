using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using identity_connect;
using identity_connect.Resources;

namespace identity_connect.Expansions
{
    public static class ClaimsExpansions
    {
        public static bool IsAdmin(this ClaimsPrincipal user) =>
            user.IsAllowed("roles.admin");

        public static bool IsOwner(this ClaimsPrincipal user, string moduleName) =>
            user.IsAllowed($"crm.{user.GetId()}.{moduleName}");

        public static int? GetId(this ClaimsPrincipal user)
        {
            var id = user.Claims.SingleOrDefault(x => x.Type == Naming.CLAIM_USER_ID);
            if (id is null || String.IsNullOrEmpty(id.Value))
                return null;

            return int.Parse(id.Value);
        }

        public static string GetServiceName(this ClaimsPrincipal service)
        {
            var id = service.Claims.SingleOrDefault(x => x.Type == Naming.CLAIM_SERVICE_NAME);
            if (id is null || String.IsNullOrEmpty(id.Value))
                return null;

            return id.Value;
        }

        public static string GetServiceExternalId(this ClaimsPrincipal service)
        {
            var id = service.Claims.SingleOrDefault(x => x.Type == Naming.CLAIM_SERVICE_EXTERNAL_ID);
            if (id is null || String.IsNullOrEmpty(id.Value))
                return null;

            return id.Value;
        }

        public static bool IsAllowed(this ClaimsPrincipal user, string permission) =>
            user.Permissions()?.Any(x => permission.Check(x.Value) > -1) == true;

        private static IEnumerable<Claim> Permissions(this ClaimsPrincipal user) =>
            user.Claims.Where(x => x.Type == Naming.CLAIM_PERMISSION);

        /// <summary>
        /// Проверить, есть ли доступ у Permission в правах value
        /// </summary>
        /// <returns>Вернёт -2, если сравнение невозможно. Вернёт 1, если указанное значение меньше значения Permission. Вернёт -1, если наоборот и 0 если они равны</returns>
        private static int Check(this string value, string permission)
        {
            var permissionObjects = GetPermissionObjects(permission);
            var valueObjects = GetPermissionObjects(value);

            if (valueObjects.Length < permissionObjects.Length)
                return IsCorrect(permissionObjects, valueObjects) ? -1 : -2;
            if (valueObjects.Length > permissionObjects.Length)
                return IsCorrect(valueObjects, permissionObjects) ? 1 : -2;
            return IsCorrect(valueObjects, permissionObjects) ? 0 : -2;
        }

        private static bool IsCorrect(string[] firstArray, string[] secondArray)
        {
            for (int i = 0; i < firstArray.Length; i++)
            {
                if (secondArray.Length - 1 < i || secondArray[i] == "*")
                    break;
                if (firstArray[i] != secondArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Получить список элементов значения заданного Permission
        /// </summary>
        /// <returns></returns>
        private static string[] GetPermissionObjects(string value) =>
            value.Split('.').Where(x => x != "*").ToArray();

        /// <summary>
        /// Получить родительский Permission, предка индекса index
        /// </summary>
        /// <param name="index">уровень погружения</param>
        /// <returns></returns>
        private static string GetParent(int index, string permission)
        {
            var permissionObjects = permission.Split('.');
            if (permissionObjects[permissionObjects.Length - 1] == "*")
                Array.Resize(ref permissionObjects, permissionObjects.Length - index);
            else
                Array.Resize(ref permissionObjects, permissionObjects.Length - index + 1);
            permissionObjects[permissionObjects.Length - 1] = "*";

            return String.Join('.', permissionObjects);
        }
    }
}
