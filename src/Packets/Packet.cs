﻿using DotNetty.Buffers;
using nMqtt.Protocol;
using System;
using System.Collections.Generic;

namespace nMqtt.Packets
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public abstract class Packet
    {
        /// <summary>
        /// 固定报头
        /// </summary>
        public FixedHeader FixedHeader { get; set; }

        public Packet()
        {
            var att = (PacketTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PacketTypeAttribute));
            FixedHeader = new FixedHeader(att.PacketType);
        }

        public Packet(PacketType msgType) => FixedHeader = new FixedHeader(msgType);

        public virtual void Encode(IByteBuffer buffer) => FixedHeader.WriteTo(buffer);

        public virtual void Decode(IByteBuffer buffer) { }
    }

    /// <summary>
    /// 固定报头
    /// </summary>
    public class FixedHeader
    {
        /// <summary>
        /// 报文类型
        /// </summary>
        public PacketType PacketType { get; set; }
        /// <summary>
        /// 重发标志
        /// </summary>
        public bool Dup { get; set; }
        /// <summary>
        /// 服务质量等级
        /// </summary>
        public MqttQos Qos { get; set; }
        /// <summary>
        /// 保留标志
        /// </summary>
        public bool Retain { get; set; }
        /// <summary>
        /// 剩余长度
        /// </summary>
        public int RemaingLength { get; set; }

        public FixedHeader(PacketType packetType)
        {
            PacketType = packetType;
        }

        public FixedHeader(IByteBuffer buffer)
        {
            var byte1 = buffer.ReadByte();
            PacketType = (PacketType)((byte1 & 0xf0) >> 4);
            Dup = ((byte1 & 0x08) >> 3) > 0;
            Qos = (MqttQos)((byte1 & 0x06) >> 1);
            Retain = (byte1 & 0x01) > 0;

            RemaingLength = DecodeRemainingLength(buffer);
        }

        public void WriteTo(IByteBuffer buffer)
        {
            var flags = (byte)PacketType << 4;
            flags |= Dup.ToByte() << 3;
            flags |= (byte)Qos << 1;
            flags |= Retain.ToByte();

            buffer.WriteByte((byte)flags);
            buffer.WriteBytes(EncodeLength(RemaingLength));
        }

        internal static byte[] EncodeLength(int length)
        {
            var result = new List<byte>();
            do
            {
                var digit = (byte)(length % 0x80);
                length /= 0x80;
                if (length > 0)
                    digit |= 0x80;
                result.Add(digit);
            } while (length > 0);

            return result.ToArray();
        }

        internal static int DecodeRemainingLength(IByteBuffer buffer)
        {
            byte encodedByte;
            var multiplier = 1;
            var remainingLength = 0;
            do
            {
                encodedByte = buffer.ReadByte();
                remainingLength += (encodedByte & 0x7f) * multiplier;
                multiplier *= 0x80;
            } while ((encodedByte & 0x80) != 0);

            return remainingLength;
        }
    }

    /// <summary>
    /// 报文标识符
    /// </summary>
    public interface IMqttPacketIdentifier
    {
        ushort PacketIdentifier { get; set; }
    }
}
