/*
 *  Sharing variables between C and C#
 *  -   Pass pointers to variables in C# to C so C can modify and use them
 *  -   Have C keep track of the variables it have been given and let C#
 *      know what variables have been given to C
 *
 *  In theory this should work
 */


using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;

namespace LinC {
    static class Types {
        public const int INTEGER    = 1;
        public const int FLOAT      = 2;
        public const int DOUBLE     = 4;
        public const int BOOL       = 8;
    }

    class Entry {
        [StructLayout(LayoutKind.Sequential, Pack=1)]
        unsafe public struct vars_t {
            public int num_vars;
            public unsafe void** vars;
            public unsafe int* types;
            readonly unsafe int size;
        };

        // Functions are named in the context of the C bridge
        [DllImport(@"./c_bridge.so")]
        public static extern unsafe vars_t* bridge_init(string lua_file);
        [DllImport(@"./c_bridge.so")]
        public static extern unsafe void receive_data(void* data, int type, vars_t* vars_list);

        // Wrapper methods
        static unsafe ref vars_t InitBridge(string LuaFilePath) {
            string file_contents = File.ReadAllText(LuaFilePath);
            vars_t* new_vars_list = bridge_init(file_contents);
            return ref Unsafe.AsRef<vars_t>(new_vars_list);
        }

        static unsafe void SendVar<T>(ref T data, int type, ref vars_t var_list) where T: unmanaged {
            fixed (T* data_ref = &data) {
                fixed (vars_t* var_list_ref = &var_list) {
                    receive_data(data_ref, 1, var_list_ref);
                };
            };
        }
        
        static void Main(string[] args) {
            try {
                vars_t VarsList = InitBridge(args[0]);

                int test1 = 5;
                int test2 = 8;
                SendVar(ref test1, Types.INTEGER, ref VarsList);
                SendVar(ref test2, Types.INTEGER, ref VarsList);
            } catch (IndexOutOfRangeException e) {
                Console.WriteLine("Error: Must pass path to a Lua file as an CLI arg");
            } 
        }
    }
}
