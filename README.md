# nMqtt
```
c# mqtt 3.1.1 client
```

## Install

`PM> Install-Package nMqtt2`


## Examples
```c#
using System;
using System.Text;
using Microsoft.Extensions.Logging;
using nMqtt.Messages;

namespace nMqtt.Test
{
    class Program
    {
        static ILogger _logger;
        static MqttClient _client;

        static void Main(string[] args)
        {
            var loggerFactory = new LoggerFactory()
                .AddConsole();
            _logger = loggerFactory.CreateLogger<Program>();

            _client = new MqttClient("127.0.0.1", logger: _logger);
            _client.OnMessageReceived += OnMessageReceived;
            _client.ConnectAsync().Wait();

            while (Console.ReadLine() != "c")
            {
                _client.Publish("/World", Encoding.UTF8.GetBytes("测试发送消息"), Qos.AtLeastOnce);
            }

            Console.ReadKey();
        }

        static void OnMessageReceived(MqttMessage message)
        {
            switch (message)
            {
                case ConnAckMessage msg:
                    _logger.LogInformation("---- OnConnAck");
                    _client.Subscribe("/World");
                    break;

                case SubscribeAckMessage msg:
                    _logger.LogInformation("---- OnSubAck");
                    break;

                case PublishMessage msg:
                    _logger.LogInformation("---- OnMessageReceived");
                    _logger.LogInformation(@"topic:{0} data:{1}", msg.TopicName, Encoding.UTF8.GetString(msg.Payload));
                    break;

                default:
                    break;
            }
        }
    }
}
```


## EMQ  百万级分布式开源物联网MQTT消息服务器
http://www.emqtt.com/

***************************

## 概览
#### MQTT是一个轻量的发布订阅模式消息传输协议，专门针对低带宽和不稳定网络环境的物联网应用设计
```
MQTT 官网:           http://mqtt.org
MQTT V3.1.1协议规范: http://docs.oasis-open.org/mqtt/mqtt/v3.1.1/os/mqtt-v3.1.1-os.html
MQTT 协议英文版:     http://docs.oasis-open.org/mqtt/mqtt/v3.1.1/mqtt-v3.1.1.html
MQTT 协议中文版:      https://mcxiaoke.gitbooks.io/mqtt-cn/content/

```

## 特点
```
1.开放消息协议，简单易实现
2.发布订阅模式，一对多消息发布
3.基于TCP/IP网络连接
4.1字节固定报头，2字节心跳报文，报文结构紧凑
5.消息QoS支持，可靠传输保证
```

## 应用
#### MQTT协议广泛应用于物联网、移动互联网、智能硬件、车联网、电力能源等领域。
```
1.物联网M2M通信，物联网大数据采集
2.Android消息推送，WEB消息推送
3.移动即时消息，例如Facebook Messenger
4.智能硬件、智能家具、智能电器
5.车联网通信，电动车站桩采集
6.智慧城市、远程医疗、远程教育
7.电力、石油与能源等行业市场
```

## MQTT基于主题(Topic)消息路由
```
chat/room/1
sensor/10/temperature
sensor/+/temperature
$SYS/broker/metrics/packets/received
$SYS/broker/metrics/#
```

#### 主题(Topic)通过’/’分割层级，支持’+’, ‘#’通配符:
```
'+': 表示通配一个层级，例如a/+，匹配a/x, a/y
'#': 表示通配多个层级，例如a/#，匹配a/x, a/b/c/d
```

#### 订阅者与发布者之间通过主题路由消息进行通信，例如采用mosquitto命令行发布订阅消息:
```
mosquitto_sub -t a/b/+ -q 1
mosquitto_pub -t a/b/c -m hello -q 1
```

#### 注解
`订阅者可以订阅含通配符主题，但发布者不允许向含通配符主题发布消息。`

***************************

## MQTT V3.1.1协议报文
#### 报文结构
```
固定报头(Fixed header)
可变报头(Variable header)
报文有效载荷(Payload)
```

#### 固定报头
```
| Bit   |  7 |  6 |  5 |  4 |  3 |  2 |  1 |  0 |
| ----- |----|----|----|----|----|----|----|----|
|byte1  | MQTT Packet type  | Flags
|byte2… | Remaining Length
```


#### 报文类型
| 类型名称  | 类型值 | 报文说明 |
| -------- | ---- | ------------- |
|CONNECT	|1	  |发起连接       |
|CONNACK	|2	  |连接回执       |
|PUBLISH	|3	  |发布消息       |
|PUBACK	    |4	  |发布回执       |
|PUBREC	    |5	  |QoS2消息回执	  |
|PUBREL	    |6	  |QoS2消息释放	  |
|PUBCOMP	|7	  |QoS2消息完成	  |
|SUBSCRIBE  |8	  |订阅主题		  |
|SUBACK	    |9	  |订阅回执		  |
|UNSUBSCRIBE|10	  |取消订阅		  |
|UNSUBACK	|11	  |取消订阅回执	  |
|PINGREQ	|12	  |PING请求		  |
|PINGRESP	|13	  |PING响应		  |
|DISCONNECT	|14	  |断开连接		  |
