西门子PLC的PPI协议监控小工具

[TOC]

# 前言
  1) 使用 Dotnet 8 + WinForm 开发。  
  2) 当现有西门子PLC与HMI通过PPI通讯、且没有变量表情况下，可通过本工具辅助迅速获取大部分变量信息。 
  3)  对PPI协议的解析, 官方文档语焉不详，而网上的解读文章也不太清楚, 这里经过实测解决了不少问题后，第一版本终于可以使用了。
  4) 这第一个版本功能简单, 后续将完善协议解析、界面重构。
  5) 如试用后有好的建议, 可发邮件 blueskit@outlook.com

<img width="1665" height="938" alt="image" src="https://github.com/user-attachments/assets/5aa3ef89-9472-425e-86b4-271ea395e645" />


# RELEASE下载
  下载的程序运行需要 Windows7/10/11,并已经安装了 DotNet8 runtime.

  https://github.com/blueskit/siemens-s7-ppi-monitor/releases

## 使用方法:
  1) 在PC上可使用 USB-RS485转换器。 
  2) 在PLC和HMI通讯的RS485串口电缆上，跨接上述 RS485(仅用作读取，不做任何写操作)。 
  3) 运行本软件后，可显示串口上往返的各变量读写。 
  4) 因无法确定变量类型，全部按可能的数据类型解读并显示。 
