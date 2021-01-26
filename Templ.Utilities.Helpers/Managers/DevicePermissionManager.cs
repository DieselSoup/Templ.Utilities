using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;


namespace Templ.Utilities.Managers
{
    public class DevicePermissionManager
    {

        private class RequestPermissionQueueItem
        {
            public Permission Permission { get; set; }
            public Action<Dictionary<Permission, PermissionStatus>> OnPermissionResultCallBack { get; set; }
        }

        public static DevicePermissionManager Instance = new DevicePermissionManager();

        private List<RequestPermissionQueueItem> permissionsToBeRequested = new List<RequestPermissionQueueItem>();
        private bool isBusy;
        private int currentPermissionRequest = -1;

        private DevicePermissionManager()
        {

        }

        [Obsolete]
        private void ExecuteNextPendingPermissionRequests()
        {
            RequestPermissionAsync(permissionsToBeRequested[currentPermissionRequest].Permission,
                permissionsToBeRequested[currentPermissionRequest].OnPermissionResultCallBack);

        }

        [Obsolete]
        public async void RequestPermissionAsync(Permission permission, Action<Dictionary<Permission, PermissionStatus>> onPermissionResult)
        {

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status == PermissionStatus.Granted)
            {
                Dictionary<Permission, PermissionStatus> result = new Dictionary<Permission, PermissionStatus>
            {
                { permission, PermissionStatus.Granted }
            };
                onPermissionResult?.Invoke(result);
                return;
            }

            if (isBusy)
            {
                RequestPermissionQueueItem requestPermissionQueueItem = new RequestPermissionQueueItem
                {
                    Permission = permission,
                    OnPermissionResultCallBack = onPermissionResult
                };
                permissionsToBeRequested.Add(requestPermissionQueueItem);
                return;
            }

            isBusy = true;
            Dictionary<Permission, PermissionStatus> results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
            onPermissionResult?.Invoke(results);
            isBusy = false;

            currentPermissionRequest++;
            if (currentPermissionRequest < permissionsToBeRequested.Count)
            {

                ExecuteNextPendingPermissionRequests();

            }
            else
            {
                currentPermissionRequest = -1;
                permissionsToBeRequested = new List<RequestPermissionQueueItem>();
            }

        }
    }
}
