﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace Realms
{
    internal abstract class NotifiableObjectHandleBase : RealmHandle, IThreadConfinedHandle
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CollectionChangeSet
        {
            public MarshaledVector<IntPtr> Deletions;
            public MarshaledVector<IntPtr> Insertions;
            public MarshaledVector<IntPtr> Modifications;

            [StructLayout(LayoutKind.Sequential)]
            public struct Move
            {
                public IntPtr From;
                public IntPtr To;
            }

            public MarshaledVector<Move> Moves;
            public MarshaledVector<IntPtr> Properties;
        }

        private static class NativeMethods
        {
            [DllImport(InteropConfig.DLL_NAME, EntryPoint = "object_destroy_notificationtoken", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr destroy_notificationtoken(IntPtr token, out NativeException ex);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NotificationCallbackDelegate(IntPtr managedHandle, IntPtr changes, IntPtr notificationException);

        protected NotifiableObjectHandleBase(RealmHandle root) : base(root)
        {
        }

        public IntPtr DestroyNotificationToken(IntPtr token)
        {
            NativeException nativeException;
            var result = NativeMethods.destroy_notificationtoken(token, out nativeException);
            nativeException.ThrowIfNecessary();
            return result;
        }

        public abstract IntPtr AddNotificationCallback(IntPtr managedObjectHandle, NotificationCallbackDelegate callback);

        public abstract ThreadSafeReferenceHandle GetThreadSafeReference();
    }
}
