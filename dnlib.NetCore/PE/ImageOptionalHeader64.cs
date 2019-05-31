// dnlib: See LICENSE.txt for more info

using dnlib.NetCore.IO;
using System;

namespace dnlib.NetCore.PE
{
    /// <summary>
    /// Represents the IMAGE_OPTIONAL_HEADER64 PE section
    /// </summary>
    public sealed class ImageOptionalHeader64 : FileSection, IImageOptionalHeader
    {
        private readonly ushort magic;
        private readonly byte majorLinkerVersion;
        private readonly byte minorLinkerVersion;
        private readonly uint sizeOfCode;
        private readonly uint sizeOfInitializedData;
        private readonly uint sizeOfUninitializedData;
        private readonly RVA addressOfEntryPoint;
        private readonly RVA baseOfCode;
        private readonly ulong imageBase;
        private readonly uint sectionAlignment;
        private readonly uint fileAlignment;
        private readonly ushort majorOperatingSystemVersion;
        private readonly ushort minorOperatingSystemVersion;
        private readonly ushort majorImageVersion;
        private readonly ushort minorImageVersion;
        private readonly ushort majorSubsystemVersion;
        private readonly ushort minorSubsystemVersion;
        private readonly uint win32VersionValue;
        private readonly uint sizeOfImage;
        private readonly uint sizeOfHeaders;
        private readonly uint checkSum;
        private readonly Subsystem subsystem;
        private readonly DllCharacteristics dllCharacteristics;
        private readonly ulong sizeOfStackReserve;
        private readonly ulong sizeOfStackCommit;
        private readonly ulong sizeOfHeapReserve;
        private readonly ulong sizeOfHeapCommit;
        private readonly uint loaderFlags;
        private readonly uint numberOfRvaAndSizes;
        private readonly ImageDataDirectory[] dataDirectories = new ImageDataDirectory[16];

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.Magic field
        /// </summary>
        public ushort Magic
        {
            get { return magic; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MajorLinkerVersion field
        /// </summary>
        public byte MajorLinkerVersion
        {
            get { return majorLinkerVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MinorLinkerVersion field
        /// </summary>
        public byte MinorLinkerVersion
        {
            get { return minorLinkerVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfCode field
        /// </summary>
        public uint SizeOfCode
        {
            get { return sizeOfCode; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfInitializedData field
        /// </summary>
        public uint SizeOfInitializedData
        {
            get { return sizeOfInitializedData; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfUninitializedData field
        /// </summary>
        public uint SizeOfUninitializedData
        {
            get { return sizeOfUninitializedData; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.AddressOfEntryPoint field
        /// </summary>
        public RVA AddressOfEntryPoint
        {
            get { return addressOfEntryPoint; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.BaseOfCode field
        /// </summary>
        public RVA BaseOfCode
        {
            get { return baseOfCode; }
        }

        /// <summary>
        /// Returns 0 since BaseOfData is not present in IMAGE_OPTIONAL_HEADER64
        /// </summary>
        public RVA BaseOfData
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.ImageBase field
        /// </summary>
        public ulong ImageBase
        {
            get { return imageBase; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SectionAlignment field
        /// </summary>
        public uint SectionAlignment
        {
            get { return sectionAlignment; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.FileAlignment field
        /// </summary>
        public uint FileAlignment
        {
            get { return fileAlignment; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MajorOperatingSystemVersion field
        /// </summary>
        public ushort MajorOperatingSystemVersion
        {
            get { return majorOperatingSystemVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MinorOperatingSystemVersion field
        /// </summary>
        public ushort MinorOperatingSystemVersion
        {
            get { return minorOperatingSystemVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MajorImageVersion field
        /// </summary>
        public ushort MajorImageVersion
        {
            get { return majorImageVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MinorImageVersion field
        /// </summary>
        public ushort MinorImageVersion
        {
            get { return minorImageVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MajorSubsystemVersion field
        /// </summary>
        public ushort MajorSubsystemVersion
        {
            get { return majorSubsystemVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.MinorSubsystemVersion field
        /// </summary>
        public ushort MinorSubsystemVersion
        {
            get { return minorSubsystemVersion; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.Win32VersionValue field
        /// </summary>
        public uint Win32VersionValue
        {
            get { return win32VersionValue; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfImage field
        /// </summary>
        public uint SizeOfImage
        {
            get { return sizeOfImage; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfHeaders field
        /// </summary>
        public uint SizeOfHeaders
        {
            get { return sizeOfHeaders; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.CheckSum field
        /// </summary>
        public uint CheckSum
        {
            get { return checkSum; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.Subsystem field
        /// </summary>
        public Subsystem Subsystem
        {
            get { return subsystem; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.DllCharacteristics field
        /// </summary>
        public DllCharacteristics DllCharacteristics
        {
            get { return dllCharacteristics; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfStackReserve field
        /// </summary>
        public ulong SizeOfStackReserve
        {
            get { return sizeOfStackReserve; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfStackCommit field
        /// </summary>
        public ulong SizeOfStackCommit
        {
            get { return sizeOfStackCommit; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfHeapReserve field
        /// </summary>
        public ulong SizeOfHeapReserve
        {
            get { return sizeOfHeapReserve; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.SizeOfHeapCommit field
        /// </summary>
        public ulong SizeOfHeapCommit
        {
            get { return sizeOfHeapCommit; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.LoaderFlags field
        /// </summary>
        public uint LoaderFlags
        {
            get { return loaderFlags; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.NumberOfRvaAndSizes field
        /// </summary>
        public uint NumberOfRvaAndSizes
        {
            get { return numberOfRvaAndSizes; }
        }

        /// <summary>
        /// Returns the IMAGE_OPTIONAL_HEADER64.DataDirectories field
        /// </summary>
        public ImageDataDirectory[] DataDirectories
        {
            get { return dataDirectories; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">PE file reader pointing to the start of this section</param>
        /// <param name="totalSize">Total size of this optional header (from the file header)</param>
        /// <param name="verify">Verify section</param>
        /// <exception cref="BadImageFormatException">Thrown if verification fails</exception>
        public ImageOptionalHeader64(IImageStream reader, uint totalSize, bool verify)
        {
            if (totalSize < 0x70)
                throw new BadImageFormatException("Invalid optional header size");
            if (verify && reader.Position + totalSize > reader.Length)
                throw new BadImageFormatException("Invalid optional header size");
            SetStartOffset(reader);
            this.magic = reader.ReadUInt16();
            this.majorLinkerVersion = reader.ReadByte();
            this.minorLinkerVersion = reader.ReadByte();
            this.sizeOfCode = reader.ReadUInt32();
            this.sizeOfInitializedData = reader.ReadUInt32();
            this.sizeOfUninitializedData = reader.ReadUInt32();
            this.addressOfEntryPoint = (RVA)reader.ReadUInt32();
            this.baseOfCode = (RVA)reader.ReadUInt32();
            this.imageBase = reader.ReadUInt64();
            this.sectionAlignment = reader.ReadUInt32();
            this.fileAlignment = reader.ReadUInt32();
            this.majorOperatingSystemVersion = reader.ReadUInt16();
            this.minorOperatingSystemVersion = reader.ReadUInt16();
            this.majorImageVersion = reader.ReadUInt16();
            this.minorImageVersion = reader.ReadUInt16();
            this.majorSubsystemVersion = reader.ReadUInt16();
            this.minorSubsystemVersion = reader.ReadUInt16();
            this.win32VersionValue = reader.ReadUInt32();
            this.sizeOfImage = reader.ReadUInt32();
            this.sizeOfHeaders = reader.ReadUInt32();
            this.checkSum = reader.ReadUInt32();
            this.subsystem = (Subsystem)reader.ReadUInt16();
            this.dllCharacteristics = (DllCharacteristics)reader.ReadUInt16();
            this.sizeOfStackReserve = reader.ReadUInt64();
            this.sizeOfStackCommit = reader.ReadUInt64();
            this.sizeOfHeapReserve = reader.ReadUInt64();
            this.sizeOfHeapCommit = reader.ReadUInt64();
            this.loaderFlags = reader.ReadUInt32();
            this.numberOfRvaAndSizes = reader.ReadUInt32();
            for (int i = 0; i < dataDirectories.Length; i++)
            {
                uint len = (uint)(reader.Position - startOffset);
                if (len + 8 <= totalSize)
                    dataDirectories[i] = new ImageDataDirectory(reader, verify);
                else
                    dataDirectories[i] = new ImageDataDirectory();
            }
            reader.Position = (long)startOffset + totalSize;
            SetEndoffset(reader);
        }
    }
}