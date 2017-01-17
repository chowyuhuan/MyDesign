using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network
{
    public struct ProtocolHeader
	{
		public const uint DataSize = 16;
		public uint mCommand;
        public uint mLen;
        public uint mSeq;
		public ushort mMagic;
        public ushort mRetCode;
		public void FromBytes(byte[] bytes)
		{
			int nPos = 0;
			
			mCommand = BitConverter.ToUInt32(bytes, nPos);
			nPos += 4;
			
			mLen = BitConverter.ToUInt32(bytes, nPos);
			nPos += 4;
			
			mSeq = BitConverter.ToUInt32(bytes, nPos);
			nPos += 4;

			mMagic = BitConverter.ToUInt16(bytes, nPos);
			nPos += 2;

			mRetCode = BitConverter.ToUInt16(bytes, nPos);
            nPos += 2;
		}
		
		public void ToBytes(byte[] bytes, ref int nPos)
		{
			byte[] byBuff = null;
			
			byBuff = BitConverter.GetBytes(mCommand);
			byBuff.CopyTo(bytes, nPos);
			nPos += byBuff.Length;
			
			byBuff = BitConverter.GetBytes(mLen);
			byBuff.CopyTo(bytes, nPos);
			nPos += byBuff.Length;
			
			byBuff = BitConverter.GetBytes(mSeq);
			byBuff.CopyTo(bytes, nPos);
			nPos += byBuff.Length;
			
			byBuff = BitConverter.GetBytes(mMagic);
			byBuff.CopyTo(bytes, nPos);
			nPos += byBuff.Length;
			
			byBuff = BitConverter.GetBytes(mRetCode);
			byBuff.CopyTo(bytes, nPos);
			nPos += byBuff.Length;
		}
	}
}
