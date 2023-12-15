using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.DataHeader
{
    public class ShareMemory<T> : IDisposable where T : struct
    {
        //添加到泛型約束 where T : struct
        private string m_SharedMemoryName;
        private MemoryMappedFile m_MemoryMappedFile;
        private MemoryMappedViewAccessor m_Accessor;
        

        private readonly Mutex m_Mutex;
        private readonly int MaxThread = 100;
        private readonly int MaxDataSize = 1024 * 1024;//1MB

        public ShareMemory(string smname)
        {
            if (smname.Length>0)
            {
                m_SharedMemoryName = smname;
                m_MemoryMappedFile = MemoryMappedFile.CreateOrOpen(m_SharedMemoryName, MaxDataSize);
                m_Accessor = m_MemoryMappedFile.CreateViewAccessor(0, MaxDataSize);
                m_Mutex = new Mutex(false, m_SharedMemoryName+"Mutex");
            }
        }
        /// <summary>
        /// 回傳是否初始化
        /// </summary>
        /// <returns>是否初始化</returns>
        public bool Get_Status()
        {
            if (m_SharedMemoryName != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string get_Name()
        {
            return m_SharedMemoryName;
        }
        public void WriteData(T[] data, int nSize, int nOffset = 0)
        {
            m_Mutex.WaitOne();
            m_Accessor.WriteArray(0, data, nOffset, nSize);
            //m_Accessor.Write(0, data);
            m_Mutex.ReleaseMutex();
        }
        public T[] ReadData(int nSize, int nOffset = 0)
        {
            m_Mutex.WaitOne();

            T[] data = new T[nSize];
            m_Accessor.ReadArray(0, data, nOffset, nSize);
            //m_Accessor.Read(0, out data);
            m_Mutex.ReleaseMutex();
            return data;
        }
        public void Dispose()
        {
            m_SharedMemoryName = "";
            m_Mutex.Dispose();
            m_Accessor.Dispose();
            m_MemoryMappedFile.Dispose();
        }

    }

    struct USMHEADER
    {
    };
    struct SHAREMEMORYTHREAD
    {
        public int id;
        public int evidx;
    };
    public class USM_protype<T> : DataHeadlerBase
    {
        const uint MUTEX_MODIFY_STATE = 0x0001;
        const uint SYNCHRONIZE = 0x00100000;

        const uint FILE_MAP_READ = 0x0004;
        const uint FILE_MAP_WRITE = 0x0002;

        static readonly IntPtr INVALID_HANDLE_VALUE = IntPtr.Zero;
        const uint PAGE_READWRITE = 0x04;

        const ulong DataSize_OneMB = 1024 * 1024;
        //----------------------------------------------
        string cwmn;
        string fmn;
        string evrn;
        string evwn;

        bool FirstFlag = false;
        IntPtr EventWrote = IntPtr.Zero;
        IntPtr MutexWriting = IntPtr.Zero;
        IntPtr EventMeReading = IntPtr.Zero;
        IntPtr FileMapping = IntPtr.Zero;
        ulong ClientSize = 0;
        int MaxThreads = 0;
        IntPtr Buff = IntPtr.Zero;//buff
        private Mutex m_mutex = new Mutex();

        SHAREMEMORYTHREAD[] m_ShareMemoryThread; 

        //-----------------------------------------------------------------------------
        //1048576 =1024*1024= 1mb
        public USM_protype(string string_id = "", bool Init = false, ulong clientsz = DataSize_OneMB, int maxthread = 100)
        {
            //若鍵入ID為空
            if (string_id == "" && string_id == null)
            {
                //這裡要新增Log表示初始化失敗
                Console.WriteLine("Fail to Init ShareMemory item");
                return;
            }
            InitParam(string_id, Init, clientsz, maxthread);
        }
        ~USM_protype()
        {
            EndItem();
        }
       

        //初始化-----------------------------
        private void InitParam(string string_id, bool init = false, ulong clientsz = DataSize_OneMB, int maxthread = 100)
        {
            if (string_id == "" && string_id == null || string_id.Length == 0)
            {
                //這裡要新增Log表示初始化失敗
                Console.WriteLine("Fail to Init ShareMemory item");
                return;
            }
            //鍵入Event的關鍵字
            string xup = string_id;
            cwmn = $"{xup}_cwmn";
            evrn = $"{xup}_evrn";
            evwn = $"{xup}_evwn";
            fmn = $"{xup}_fmn";

            //確保1MB
            if (clientsz != DataSize_OneMB)
            {
                clientsz = DataSize_OneMB;
            }
            ClientSize = clientsz;//對應端點1MB
            //確保100
            if (maxthread != 100)
            {
                maxthread = 100;
            }
            MaxThreads = maxthread;//最大100線程堆疊
            if (init)//必須是true才會初始化
            {
                bool re = Initialize();
                if (!re)
                {
                    //
                    Console.WriteLine("ShareMemory:InitParam初始化失敗");
                    EndItem();

                }
            }
        }
        private bool Initialize()
        {
            EventWrote = IntPtr.Zero;
            MutexWriting = IntPtr.Zero;
            FileMapping = IntPtr.Zero;
            Buff = IntPtr.Zero;
            EventMeReading = IntPtr.Zero;
            //若線程寫入為空
            if (MutexWriting == IntPtr.Zero)
            {
                MutexWriting = CreateMutexWrite();
                if (MutexWriting == IntPtr.Zero)
                {
                    return false;//CWM初始化失敗
                }
            }
            //記憶體對映檔案
            if (FileMapping == IntPtr.Zero)
            {
                FileMapping = CreateFilesMapping();
                if (FileMapping == IntPtr.Zero)
                {
                    return false;//FM初始化失敗
                }
            }
            //寫入事件
            if (EventWrote == IntPtr.Zero)
            {
                EventWrote = CreateEventWrote();
                if (EventWrote == IntPtr.Zero)
                {
                    return false;//FM初始化失敗
                }
            }
            //傳回檔案檢視的指標
            if (Buff == IntPtr.Zero)
            {
                Buff = CreateMappedView();
                if (Buff != IntPtr.Zero)
                {
                    return false;
                }
            }
            //--------------------------------------------------------------------
            m_mutex.WaitOne();
            // 受 Mutex 保護區塊
            IntPtr headerOffset = IntPtr.Add(Buff, Marshal.SizeOf<USMHEADER>());
            SHAREMEMORYTHREAD[] m_ShareMemorythread = new SHAREMEMORYTHREAD[MaxThreads];
            // Find 
            for (int i = 0; i < MaxThreads; i++)
            {
                IntPtr currentOffset = IntPtr.Add(headerOffset, Marshal.SizeOf<SHAREMEMORYTHREAD>() * i);//對應thread 
                SHAREMEMORYTHREAD smt = Marshal.PtrToStructure<SHAREMEMORYTHREAD>(currentOffset);
                if (smt.id == 0)
                {
                    smt.id = Thread.CurrentThread.ManagedThreadId;
                    smt.evidx = (i + 1);
                    EventMeReading = CreateEventRead(i + 1);
                    if (EventMeReading == IntPtr.Zero)
                    {
                        return false;
                    }
                    break;
                }
            }
            m_mutex.ReleaseMutex();
            return true;
        }
        private IntPtr CreateMutexWrite()
        {
            IntPtr re = OpenMutex(MUTEX_MODIFY_STATE | SYNCHRONIZE, false, cwmn);
            if (re != IntPtr.Zero)
            {
                return re;
            }
            re = CreateMutex(IntPtr.Zero, false, cwmn);
            return re;
        }
        private IntPtr CreateFilesMapping()
        {
            // Try to open the map , or else create it
            FirstFlag = true;
            IntPtr re = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE, false, fmn);
            if (re != IntPtr.Zero)
            {
                //開啟成功
                FirstFlag = false;
                return re;
            }
            //--------------------------------------------------------------------------------------------------
            //   1MB*泛型大小    +    100線程*線程包含資料       +    檔頭
            //(1048576*Size(T))+(100*Size(SHAREMEMORYTHREAD))+Size(USMHEADER)
            ulong FinalSize = ClientSize * (ulong)Marshal.SizeOf(typeof(T)) +
                              (ulong)MaxThreads * (ulong)Marshal.SizeOf(typeof(SHAREMEMORYTHREAD)) + 
                              (ulong)Marshal.SizeOf(typeof(USMHEADER));
            ULARGE_INTEGER ulx = new ULARGE_INTEGER
            {
                QuadPart = FinalSize
            };
            //                                                                                                    HightPart          LowPart
            re = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, PAGE_READWRITE, (uint)(ulx.QuadPart >> 32), (uint)(ulx.QuadPart & 0xFFFFFFFF), fmn);
            if (re != IntPtr.Zero)
            {
                IntPtr Buff = MapViewOfFile(FileMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, UIntPtr.Zero);
                if (Buff != IntPtr.Zero)
                {
                    byte[] buffer = new byte[FinalSize]; // 创建一个和您的 FinalSize 大小相等的字節組
                    // 使用 Marshal.Copy 填滿緩衝區
                    Marshal.Copy(buffer, 0, Buff, buffer.Length);
                    UnmapViewOfFile(Buff);//關閉
                }
            }
            return re;
        }
        private IntPtr CreateEventWrote()
        {
            string n = $"{evwn}";
            IntPtr re = CreateEvent(IntPtr.Zero, false, false, n);
            return re;
        }
        private IntPtr CreateMappedView()
        {
            IntPtr re;
            re = MapViewOfFile(FileMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, UIntPtr.Zero);
            return re;
        }
        private IntPtr CreateEventRead(int evidx)
        {
            string n = $"{evrn}{evidx}";
            IntPtr re = CreateEvent(IntPtr.Zero, true, true, n);
            return re;
        }
        //解構------------------------------
        private void EndItem()
        {
            // Remove the ID from the thread
            if (Buff != IntPtr.Zero)
            {
                m_mutex.WaitOne();
                IntPtr ptr = IntPtr.Add(Buff, Marshal.SizeOf<USMHEADER>());
                SHAREMEMORYTHREAD[] tmp_smthread = new SHAREMEMORYTHREAD[MaxThreads];

                for (int y = 0; y < MaxThreads; y++)
                {
                    IntPtr itemPtr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(SHAREMEMORYTHREAD)) * y);
                    tmp_smthread[y] = Marshal.PtrToStructure<SHAREMEMORYTHREAD>(itemPtr);
                }
                // Find 
                for (int i = 0; i < MaxThreads; i++)
                {
                    SHAREMEMORYTHREAD smt = tmp_smthread[i];
                    int myid = Thread.CurrentThread.ManagedThreadId;
                    if (smt.id == myid)
                    {
                        //清零
                        smt.id = 0;
                        smt.evidx = 0;
                        break;
                    }
                }
                for (int y = 0; y < MaxThreads; y++)
                {
                    IntPtr itemPtr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(SHAREMEMORYTHREAD)) * y);
                    Marshal.StructureToPtr(tmp_smthread[y], itemPtr, false);
                }
                UnmapViewOfFile(Buff);
               
                m_mutex.ReleaseMutex();
            }

            if (EventWrote!= IntPtr.Zero)
            {
                CloseHandle(EventWrote);
                EventWrote = IntPtr.Zero;
            }
            if (FileMapping != IntPtr.Zero)
            {
                CloseHandle(FileMapping);
                FileMapping = IntPtr.Zero;
            }
            if (EventMeReading != IntPtr.Zero)
            {
                CloseHandle(EventMeReading);
                EventMeReading = IntPtr.Zero;
            }           
            if (MutexWriting != IntPtr.Zero)
            {
                CloseHandle(MutexWriting);
                MutexWriting = IntPtr.Zero;
            }
        }
        //讀取------------------------------










    }
}
