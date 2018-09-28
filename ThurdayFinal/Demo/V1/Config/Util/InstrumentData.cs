// Copyright 2018 Thermo Fisher Scientific Inc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Dionex.Chromeleon.DDK;

namespace MyCompany
{
    /// <summary>Instrument Data</summary>
    public class InstrumentData
    {
#pragma warning disable 1591
        public static readonly int MaxInstrumentCount = (int)InstrumentID.MaxNumberInstruments;
        public InstrumentID ID { get; private set; }
        public long BitMap { [DebuggerStepThrough] get { return (long)ID; } }
        public string BitMapBinary { [DebuggerStepThrough] get { return GetBitMapBinary(BitMap); } }
#pragma warning restore 1591

        private readonly Func<InstrumentID, string> m_GetInstrumentName;

        /// <summary>Initializes a new instance of the <see cref="InstrumentData"/> class.</summary>
        public InstrumentData(InstrumentID id, Func<InstrumentID, string> getInstrumentName)
        {
            if (id == InstrumentID.None)
                throw new ArgumentException("Parameter instrument id cannot be " + id.ToString());
            if (getInstrumentName == null)
                throw new ArgumentNullException("getInstrumentName");

            ID = id;
            m_GetInstrumentName = getInstrumentName;
        }

        /// <summary>Gets the name.</summary>
        public string Name
        {
            get
            {
                return m_GetInstrumentName(ID);
            }
        }

        /// <summary>Returns the bit map as binary: 0000000000000011 - this translates to 2 instruments with Ids 0001 and 0010</summary>
        public static string GetBitMapBinary(long value)
        {
            return Convert.ToString(value, 2).PadLeft(MaxInstrumentCount, '0');
        }

        /// <summary>Gets the number of instruments (bits) from the instrumentsMap (instrumentsMap = 1001 means 2 instruments).</summary>
        public static int GetInstrumentsCount(long instrumentsMap)
        {
            ulong number = (ulong)instrumentsMap;

            if (number == 0)  // It works and without this
            {
                return 0;
            }

            int result = 0;
            if ((number & 1) != 0)
            {
                result++;
            }

            do
            {
                number >>= 1;
                if ((number & 1) != 0)
                {
                    result++;
                }
            }
            while (number != 0);

            return result;
        }

        /// <summary>Gets the configuration folder.</summary>
        public string ConfigFolder
        {
            get
            {
                string instrumentName = Name;
                string result = GetConfigFolder(instrumentName);
                return result;
            }
        }

        /// <summary>Gets the configuration folder.</summary>
        public static string GetConfigFolder(string instrumentName)
        {
            if (string.IsNullOrEmpty(instrumentName))
                throw new ArgumentNullException("instrumentName");

            string programDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);  // C:\ProgramData
            string instrumentsFolder = Path.Combine(programDataFolder, @"Dionex\Chromeleon\Instruments");           // C:\ProgramData\Dionex\Chromeleon\Instruments
            string result = Path.Combine(instrumentsFolder, instrumentName);                                        // C:\ProgramData\Dionex\Chromeleon\Instruments\<instrumentName>
            return result;
        }

        /// <summary>Gets the configuration folder + the fileName.</summary>
        public string GetFullFileName(string fileName)
        {
            string instrumentName = Name;
            string result = GetFullFileName(instrumentName, fileName);
            return result;
        }

        /// <summary>Gets the configuration folder + the fileName.</summary>
        public static string GetFullFileName(string instrumentName, string fileName)
        {
            if (string.IsNullOrEmpty(instrumentName))
                throw new ArgumentNullException("instrumentName");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            string folder = GetConfigFolder(instrumentName);  // C:\ProgramData\Dionex\Chromeleon\Instruments\<instrumentName>
            string result = Path.Combine(folder, fileName);   // C:\ProgramData\Dionex\Chromeleon\Instruments\<instrumentName>\<fileName>
            return result;
        }
    }

    /// <summary>Instrument Data List</summary>
    public class InstrumentDataList : IEnumerable<InstrumentData>
    {
        /// <summary>Gets the instruments map.</summary>
        public long InstrumentsMap { get; private set; }

        private readonly List<InstrumentData> m_Instruments = new List<InstrumentData>();

        /// <summary>Initializes a new instance of the <see cref="InstrumentDataList"/> class.</summary>
        public InstrumentDataList()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InstrumentDataList"/> class.</summary>
        public InstrumentDataList(IDDK ddk, long instrumentsMap)
        {
            Init(ddk, instrumentsMap);
        }

        /// <summary>Initializes a new instance of the <see cref="InstrumentDataList"/> class. Adds all instruments available ordered by name.</summary>
        public InstrumentDataList(IInstrumentInfo instrumentInfo)
        {
            Init(instrumentInfo);
        }

        /// <summary>Initializes a new instance of the <see cref="InstrumentDataList"/> class.</summary>
        public InstrumentDataList(IInstrumentInfo instrumentInfo, long instrumentsMap)
        {
            if (instrumentInfo == null)
                throw new ArgumentNullException("instrumentInfo");
            Init(instrumentInfo.GetInstrumentName, instrumentsMap);
        }

        /// <summary>Initializes the data with all instruments available ordered by name.</summary>
        public void Init(IInstrumentInfo instrumentInfo)
        {
            if (instrumentInfo == null)
                throw new ArgumentNullException("instrumentInfo");
            long instrumentsMap = instrumentInfo.InstrumentMap;
            Init(instrumentInfo.GetInstrumentName, instrumentsMap);
            SortByName();
        }

        /// <summary>Initializes the data with the specified instruments in the instrumentsMap parameter.</summary>
        public void Init(IDDK ddk, long instrumentsMap)
        {
            if (ddk == null)
                throw new ArgumentNullException("ddk");
            Init(ddk.GetInstrumentName, instrumentsMap);
        }

        private void Init(Func<InstrumentID, string> getInstrumentName, long instrumentsMap)
        {
            if (getInstrumentName == null)
                throw new ArgumentNullException("getInstrumentName");

            InstrumentsMap = instrumentsMap;
            m_Instruments.Clear();

            if (instrumentsMap == (long)InstrumentID.None)
            {
                return;
            }

            for (int i = 0; i < InstrumentData.MaxInstrumentCount; i++)
            {
                long instrumentIdNumber = 1 << i;  // Bit Map
                //Trace.WriteLine(instrumentIdNumber.ToString("N0").PadLeft(6) + " = " + Convert.ToString(instrumentIdNumber, 2).PadLeft(InstrumentData.MaxInstrumentCount, '0'));

                if ((instrumentsMap & instrumentIdNumber) == instrumentIdNumber)
                {
                    InstrumentID instrumentId = (InstrumentID)instrumentIdNumber;
                    string instrumentName = getInstrumentName(instrumentId);
                    if (string.IsNullOrEmpty(instrumentName))
                    {
                        continue;
                    }
                    Add(instrumentId, getInstrumentName);
                }
            }
#if DEBUG
            if (Count < 1)
                throw new InvalidDataException("Failed to detect the instruments from the instrumentsMap = " + instrumentsMap.ToString() + " " +
                                               "InstrumentDataList Count = " + Count.ToString() + " must be >= 1");
#endif
        }

        /// <summary>Gets the <see cref="InstrumentData"/> with the specified i.</summary>
        public InstrumentData this[int i]
        {
            [DebuggerStepThrough]
            get { return m_Instruments[i]; }
        }

        /// <summary>Gets the instrument count.</summary>
        public int Count
        {
            [DebuggerStepThrough]
            get { return m_Instruments.Count; }
        }

        #region IEnumerable
        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        public IEnumerator<InstrumentData> GetEnumerator()
        {
            return m_Instruments.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Instruments.GetEnumerator();
        }
        #endregion

        /// <summary>Adds a new data if the list does not contain the id</summary>
        private void Add(InstrumentID id, Func<InstrumentID, string> getInstrumentName)
        {
            InstrumentData item = this.FirstOrDefault(instrument => instrument.ID == id);
            if (item != null)
            {
                return;
            }
            item = new InstrumentData(id, getInstrumentName);
            m_Instruments.Add(item);
        }

        /// <summary>Sort by name</summary>
        public void SortByName()
        {
            //Sort((instr1, instr2) => string.Compare(instr1.Name, instr2.Name));
            m_Instruments.Sort((instr1, instr2) => StringExtensions.NaturalCompare(instr1.Name, instr2.Name));
        }

        /// <summary>Get the names as text: name1, name2, ..., nameN</summary>
        public string GetNames()
        {
            StringBuilder sb = new StringBuilder();
            foreach (InstrumentData instrument in this)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(instrument.Name);
            }
            string result = sb.ToString();
            return result;
        }

        /// <summary>Gets the instrument by ID.</summary>
        public InstrumentData GetItem(InstrumentID instrumentID)
        {
            InstrumentData result = this.FirstOrDefault(instrument => instrument.ID == instrumentID);
            return result;
        }

        /// <summary>Gets the instrument by name.</summary>
        public InstrumentData GetItem(string instrumentName)
        {
            InstrumentData result = this.FirstOrDefault(instrument => instrument.Name == instrumentName);
            return result;
        }
    }
}
