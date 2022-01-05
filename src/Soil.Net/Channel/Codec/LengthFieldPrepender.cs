// using System;
// using System.Collections.Generic;
// using Soil.Buffers;
// using Soil.Types;

// namespace Soil.Net.Channel.Codec;

// public class LengthFieldPrepender : ChannelInboundPipe<IByteBuffer, IByteBuffer>
// {
//     private readonly int _lengthFieldLength;

//     public LengthFieldPrepender()
//         : this(4)
//     {
//     }

//     public LengthFieldPrepender(int lengthFieldLength)
//     {
//         if (lengthFieldLength != 1
//             && lengthFieldLength != 2
//             && lengthFieldLength != 4
//             && lengthFieldLength != 8)
//         {
//             throw new ArgumentOutOfRangeException(
//                 nameof(lengthFieldLength),
//                 lengthFieldLength,
//                 null);
//         }

//         _lengthFieldLength = lengthFieldLength;
//     }

//     public override Result<ChannelPipeResultType, IByteBuffer> Transform(
//         IChannelHandlerContext ctx,
//         IByteBuffer message)
//     {
//     }

//     public IByteBuffer ComputeLength(IChannelHandlerContext ctx, IByteBuffer message)
//     {
//         long length = message.ReadableBytes;

//         IByteBuffer lenByteBuf;
//         switch (_lengthFieldLength)
//         {
//             case 1:
//                 {
//                     if (length > byte.MaxValue)
//                     {
//                         throw new ArgumentException($"buffer length does not fit into a byte. length: {length}", nameof(message));
//                     }
//                     lenByteBuf = ctx.Allocator.Allocate(1)
//                         .WriteByte((byte)length);
//                     break;
//                 }
//             case 2:
//                 {
//                     if (length > short.MaxValue)
//                     {
//                         throw new ArgumentException($"buffer length does not fit into a short. length: {length}", nameof(message));
//                     }
//                     lenByteBuf = ctx.Allocator.Allocate(2)
//                         .WriteInt16((short)length);
//                     break;
//                 }
//             case 4:
//                 {
//                     if (length > int.MaxValue)
//                     {
//                         throw new ArgumentException($"buffer length does not fit into a int. length: {length}", nameof(message));
//                     }
//                     lenByteBuf = ctx.Allocator.Allocate(4)
//                         .WriteInt32((int)length);
//                     break;
//                 }
//             case 8:
//                 {
//                     if (length > long.MaxValue)
//                     {
//                         throw new ArgumentException($"buffer length does not fit into a long. length: {length}", nameof(message));
//                     }
//                     lenByteBuf = ctx.Allocator.Allocate(8)
//                         .WriteInt64(length);
//                     break;
//                 }
//             default:
//                 {
//                     throw new InvalidOperationException("Should not reach here");
//                 }
//         }

//         return lenByteBuf;
//     }
// }
