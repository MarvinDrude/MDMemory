using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MDMemorys {
    public class MDMemory {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAcess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32")]
        static extern uint GetLastError();

        private string pName;
        private Process pMain;
        private IntPtr pHandle;

        public MDMemory(string pName) {
            this.pName = pName;
            this.pMain = null;
            this.pHandle = IntPtr.Zero;

            FindProcess();
        }

        public float[,] ReadMatrixFloat(IntPtr address, int columns, int rows) {
            float[,] end = new float[rows, columns];
            byte[] array = Read(address, columns * rows * 4);
            int inc = 0;

            for(int col = 0; col < columns; col++) {
                for(int row = 0; row < rows; row++) {
                    end[row, col] = BitConverter.ToSingle(array, inc);
                    inc += 4;
                }
            }
            return end;
        }

        public bool ReadBoolean(IntPtr address) {
            return BitConverter.ToBoolean(Read(address, 1), 0);
        }

        public double ReadDouble(IntPtr address) {
            return BitConverter.ToDouble(Read(address, 8), 0);
        }

        public Int64 ReadInt64(IntPtr address) {
            return BitConverter.ToInt64(Read(address, 8), 0);
        }

        public Int32 ReadInt32(IntPtr address) {
            return BitConverter.ToInt32(Read(address, 4), 0);
        }

        public float ReadFloat(IntPtr address) {
            return BitConverter.ToSingle(Read(address, 4), 0);
        }

        public Int16 ReadString(IntPtr address) {
            return BitConverter.ToInt16(Read(address, 2), 0);
        }

        public Byte ReadByte(IntPtr address) {
            return Read(address, 1)[0];
        }

        public byte[] Read(IntPtr address, int l) {
            byte[] lpBuffer = new byte[l];
            int lpNumberOfBytesRead = 1;
            ReadProcessMemory(pHandle, address, lpBuffer, lpBuffer.Length, ref lpNumberOfBytesRead);
            return lpBuffer;
        }

        public void Write(IntPtr address, byte[] bytes) {
            int lpNumberOfBytesRead = 1;
            WriteProcessMemory(pHandle, address, bytes, bytes.Length, ref lpNumberOfBytesRead);
        }

        public void WriteFloat(IntPtr address, float value) {
            byte[] bytes = new byte[4];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteDouble(IntPtr address, double value) {
            byte[] bytes = new byte[8];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteBoolean(IntPtr address, bool value) {
            byte[] bytes = new byte[1];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteInt32(IntPtr address, Int32 value) {
            byte[] bytes = new byte[4];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteInt64(IntPtr address, Int64 value) {
            byte[] bytes = new byte[8];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteInt16(IntPtr address, Int16 value) {
            byte[] bytes = new byte[2];
            bytes = BitConverter.GetBytes(value);
            Write(address, bytes);
        }

        public void WriteByte(IntPtr address, byte value) {
            byte[] bytes = new byte[1];
            bytes[0] = value;
            Write(address, bytes);
        }

        public static bool IsProcessRunning(string name) {
            return (0 != Process.GetProcessesByName(name).Length);
        }

        public void FindProcess() {
            Process[] processes = Process.GetProcessesByName(pName);

            if(processes.Length == 0) {
                throw new Exception();
            } else {
                this.pMain = processes[0];
                this.pHandle = OpenProcess(0x1f0fff, false, this.pMain.Id);
            }
        }

        public Int64 GetModuleAddress(string module) {
            try {
                Process[] p = Process.GetProcessesByName(this.pName);

                foreach(ProcessModule m in p[0].Modules) {
                    if(m.ModuleName == module) {
                        return (Int64)m.BaseAddress;
                    }
                }
            } catch(Exception e) {
                return -1;
            }

            return -1;
        }

        public IntPtr GetWindowHandle() {
            try {
                Process[] p = Process.GetProcessesByName(this.pName);

                return p[0].MainWindowHandle;
            } catch(Exception e) {
                return IntPtr.Zero;
            }
        }
    }
}
