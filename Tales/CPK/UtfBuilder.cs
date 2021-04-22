using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.CPK {
	public class ColumnData {
		public byte Type;
		public string Name;
		public object Data;

		public override string ToString() {
			return string.Format("{0:x2} {1}{2}{3}", Type, Name, Data == null ? "" : ": ", Data == null ? "" : Data);
		}
	}

	public class RowData {
		public List<CellData> Cells;

		public override string ToString() {
			return string.Format("{0} Rows", Cells.Count);
		}
	}

	public class CellData {
		public object Data;

		public override string ToString() {
			return string.Format("{0}", Data == null ? "" : Data);
		}
	}

	public class UtfBuilder {
		public string Name;
		public List<ColumnData> Columns;
		public List<RowData> Rows;

		public UtfBuilder(DuplicatableStream stream, Endianness endian = Endianness.BigEndian) {
			long utfHeaderOffset = stream.Position;
			uint utfMagic = stream.ReadUInt32(Endianness.LittleEndian);
			if (utfMagic != 0x46545540) {
				throw new Exception("wrong UTF magic");
			}

			uint size = stream.ReadUInt32(endian); // size of the whole UTF chunk minus the magic and this value itself

			// offsets are relative to here
			long offset = stream.Position;

			uint rowsLocation = stream.ReadUInt32(endian);
			uint stringTableLocation = stream.ReadUInt32(endian);
			uint dataTableLocation = stream.ReadUInt32(endian);
			uint tableNameLocationInStringTable = stream.ReadUInt32(endian);
			ushort colcount = stream.ReadUInt16(endian);
			ushort rowwidth = stream.ReadUInt16(endian);
			uint rowcount = stream.ReadUInt32(endian);
			string tableName = stream.ReadUTF8NulltermFromLocationAndReset(offset + stringTableLocation + tableNameLocationInStringTable);

			// utf_tab calls this the 'schema'
			List<ColumnData> columns = new List<ColumnData>(colcount);
			for (int i = 0; i < colcount; ++i) {
				ColumnData col = new ColumnData();
				col.Type = stream.ReadUInt8();
				uint stroffset = stream.ReadUInt32(endian);
				col.Name = stream.ReadUTF8NulltermFromLocationAndReset(offset + stringTableLocation + stroffset);

				if ((col.Type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK) == utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT) {
					switch (col.Type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
							uint datastroffset = stream.ReadUInt32(endian);
							col.Data = stream.ReadUTF8NulltermFromLocationAndReset(offset + stringTableLocation + datastroffset);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
							col.Data = stream.ReadUInt64(endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
							uint dataOffset = stream.ReadUInt32(endian);
							uint dataSize = stream.ReadUInt32(endian);
							col.Data = stream.ReadBytesFromLocationAndReset(offset + dataTableLocation + dataOffset, dataSize);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
							col.Data = stream.ReadUInt32(endian).UIntToFloat();
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
							col.Data = stream.ReadUInt32(endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
							col.Data = stream.ReadUInt16(endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
							col.Data = stream.ReadUInt8();
							break;
						default:
							throw new Exception("unknown type for constant");
					}
				}

				columns.Add(col);
			}

			List<RowData> rows = new List<RowData>((int)rowcount);
			for (long i = 0; i < rowcount; ++i) {
				stream.Position = offset + rowsLocation + (i * rowwidth);
				RowData row = new RowData();
				row.Cells = new List<CellData>();
				for (int j = 0; j < colcount; ++j) {
					CellData cell = new CellData();
					byte type = columns[j].Type;
					switch ((type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK)) {
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_PERROW:
							switch (type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
									uint datastroffset = stream.ReadUInt32(endian);
									cell.Data = stream.ReadUTF8NulltermFromLocationAndReset(offset + stringTableLocation + datastroffset);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
									cell.Data = stream.ReadUInt64(endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
									uint dataOffset = stream.ReadUInt32(endian);
									uint dataSize = stream.ReadUInt32(endian);
									cell.Data = stream.ReadBytesFromLocationAndReset(offset + dataTableLocation + dataOffset, dataSize);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
									cell.Data = stream.ReadUInt32(endian).UIntToFloat();
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
									cell.Data = stream.ReadUInt32(endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
									cell.Data = stream.ReadUInt16(endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
									cell.Data = stream.ReadUInt8();
									break;
								default:
									throw new Exception("unknown type for value");
							}
							break;
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT:
							cell.Data = columns[j].Data;
							break;
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_ZERO:
							switch (type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
									cell.Data = "";
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
									cell.Data = (ulong)0;
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
									cell.Data = new byte[0];
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
									cell.Data = 0.0f;
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
									cell.Data = (uint)0;
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
									cell.Data = (ushort)0;
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
									cell.Data = (byte)0;
									break;
								default:
									throw new Exception("unknown type for value");
							}
							break;
						default:
							throw new Exception("unknown storage for value");
					}
					row.Cells.Add(cell);
				}
				rows.Add(row);
			}

			Name = tableName;
			Columns = columns;
			Rows = rows;

			stream.Position = utfHeaderOffset + ((long)8) + ((long)size);

			return;
		}

		public int FindColumnIndex(string columnName) {
			for (int i = 0; i < Columns.Count; ++i) {
				if (Columns[i].Name == columnName) {
					return i;
				}
			}
			return -1;
		}

		private class DataChunk {
			Stream Data = new MemoryStream();
			Dictionary<string, uint> ExistingStrings = new Dictionary<string, uint>();

			public uint Add(string s) {
				if (s == null) {
					return Add("<NULL>");
				}

				uint value;
				if (ExistingStrings.TryGetValue(s, out value)) {
					return value;
				}

				uint pos = (uint)Data.Position;
				StreamUtils.WriteUTF8Nullterm(Data, s);
				ExistingStrings.Add(s, pos);
				return pos;
			}

			public uint Add(byte[] b) {
				uint pos = (uint)Data.Position;
				Data.Write(b);
				return pos;
			}

			public Stream GetData() {
				return Data;
			}
		}

		private ushort CalculateRowWidth() {
			ushort rowwidth = 0;
			for (int j = 0; j < Columns.Count; ++j) {
				byte type = Columns[j].Type;
				switch ((type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK)) {
					case utf_tab_sharp.UtfTab.COLUMN_STORAGE_PERROW:
						switch (type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
								rowwidth += 4;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
								rowwidth += 8;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
								rowwidth += 8;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
								rowwidth += 4;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
								rowwidth += 4;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
								rowwidth += 2;
								break;
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
							case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
								rowwidth += 1;
								break;
							default:
								throw new Exception("unknown type for value");
						}
						break;
					case utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT:
					case utf_tab_sharp.UtfTab.COLUMN_STORAGE_ZERO:
						break;
					default:
						throw new Exception("unknown storage for value");
				}
			}
			return rowwidth;
		}

		public void Build(Stream s, Endianness endian = Endianness.BigEndian) {
			DataChunk stringTable = new DataChunk();
			stringTable.Add("<NULL>");
			DataChunk dataTable = new DataChunk();

			ushort colcount = (ushort)Columns.Count;
			ushort rowwidth = CalculateRowWidth();
			uint rowcount = (uint)Rows.Count;

			long baseOffset = s.Position;
			s.WriteUInt32(0x46545540, Endianness.LittleEndian);
			s.WriteUInt32(0); // size of this chunk, fill in later
			long offset = s.Position;
			s.WriteUInt32(0); // rowsLocation
			s.WriteUInt32(0); // stringTableLocation
			s.WriteUInt32(0); // dataTableLocation
			s.WriteUInt32(stringTable.Add(Name), endian);
			s.WriteUInt16(colcount, endian);
			s.WriteUInt16(rowwidth, endian);
			s.WriteUInt32(rowcount, endian);

			for (int i = 0; i < colcount; ++i) {
				ColumnData col = Columns[i];
				s.WriteUInt8(col.Type);
				s.WriteUInt32(stringTable.Add(col.Name), endian);
				if ((col.Type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK) == utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT) {
					switch (col.Type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
							s.WriteUInt32(stringTable.Add((string)col.Data), endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
							s.WriteUInt64((ulong)col.Data, endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
							byte[] b = (byte[])col.Data;
							uint dataOffset = dataTable.Add(b);
							uint dataSize = (uint)b.LongLength;
							s.WriteUInt32(dataOffset, endian);
							s.WriteUInt32(dataSize, endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
							s.WriteUInt32(BitConverter.ToUInt32(BitConverter.GetBytes(((float)col.Data)), 0), endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
							s.WriteUInt32((uint)col.Data, endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
							s.WriteUInt16((ushort)col.Data, endian);
							break;
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
						case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
							s.WriteByte((byte)col.Data);
							break;
						default:
							throw new Exception("unknown type for constant");
					}
				}
			}

			uint rowsLocation = (uint)(s.Position - offset);
			for (long i = 0; i < rowcount; ++i) {
				var row = Rows[(int)i];
				for (int j = 0; j < colcount; ++j) {
					byte type = Columns[j].Type;
					var cell = row.Cells[j];
					switch ((type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK)) {
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_PERROW:
							switch (type & utf_tab_sharp.UtfTab.COLUMN_TYPE_MASK) {
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_STRING:
									s.WriteUInt32(stringTable.Add((string)cell.Data), endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_8BYTE:
									s.WriteUInt64((ulong)cell.Data, endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_DATA:
									byte[] b = (byte[])cell.Data;
									uint dataOffset = dataTable.Add(b);
									uint dataSize = (uint)b.LongLength;
									s.WriteUInt32(dataOffset, endian);
									s.WriteUInt32(dataSize, endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_FLOAT:
									s.WriteUInt32(BitConverter.ToUInt32(BitConverter.GetBytes(((float)cell.Data)), 0), endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_4BYTE:
									s.WriteUInt32((uint)cell.Data, endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_2BYTE:
									s.WriteUInt16((ushort)cell.Data, endian);
									break;
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE2:
								case utf_tab_sharp.UtfTab.COLUMN_TYPE_1BYTE:
									s.WriteByte((byte)cell.Data);
									break;
								default:
									throw new Exception("unknown type for value");
							}
							break;
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT:
						case utf_tab_sharp.UtfTab.COLUMN_STORAGE_ZERO:
							break;
						default:
							throw new Exception("unknown storage for value");
					}
				}
			}

			var stringTableStream = stringTable.GetData();
			stringTableStream.Position = 0;
			uint stringTableLocation = (uint)(s.Position - offset);
			StreamUtils.CopyStream(stringTableStream, s);
			s.WriteAlign(8, 0, offset);

			var dataTableStream = dataTable.GetData();
			dataTableStream.Position = 0;
			uint dataTableLocation = (uint)(s.Position - offset);
			StreamUtils.CopyStream(dataTableStream, s);
			s.WriteAlign(8, 0, offset);

			uint size = (uint)(s.Position - offset);
			long endpos = s.Position;
			s.Position = baseOffset + 4;
			s.WriteUInt32(size, endian);
			s.WriteUInt32(rowsLocation, endian);
			s.WriteUInt32(stringTableLocation, endian);
			s.WriteUInt32(dataTableLocation, endian);

			s.Position = endpos;

			return;
		}
	}
}
