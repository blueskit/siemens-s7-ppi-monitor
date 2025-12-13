西门子PLC的PPI协议监控小工具
Siemens PLC PPI Protocol Monitoring Tool

[TOC]

# 前言 Preface
  1) 使用 Dotnet 8 + WinForm 开发。  
  2) 当现有西门子PLC与HMI通过PPI通讯、且没有变量表情况下，可通过本工具辅助迅速获取大部分变量信息。 
  3)  对PPI协议的解析, 官方文档语焉不详，而网上的解读文章也不太清楚, 这里经过实测解决了不少问题后，第一版本终于可以使用了。
  4) 这第一个版本功能简单, 后续将完善协议解析、界面重构。
  5) 如试用后有好的建议, 可发邮件 blueskit@outlook.com

Developed with Dotnet 8 + WinForm.
When an existing Siemens PLC communicates with HMI via PPI and no variable table is available, this tool can assist in quickly acquiring most variable information.
The official documentation provides only vague descriptions of PPI protocol parsing, and relevant interpretations online are also unclear. After resolving numerous issues through actual testing, the first version is finally ready for use.
This initial version has simple functions; subsequent iterations will improve protocol parsing and reconstruct the interface.
If you have any valuable suggestions after trial use, please send an email to blueskit@outlook.com.

<img width="1381" height="797" alt="image" src="https://github.com/user-attachments/assets/b23cf01f-a6ab-4eb3-92a0-a6c1ac202dcd" />

<p>
  
</p>

<img width="1387" height="801" alt="image" src="https://github.com/user-attachments/assets/bbcb9f7f-6a39-4768-9f5f-e9004c2e7cff" />

# RELEASE
  System Requirements: The downloaded program requires Windows 7/10/11 operating system with DotNet8 runtime pre-installed.
  下载的程序运行需要 Windows7/10/11,并已经安装了 DotNet8 runtime.

## Download Link
   https://github.com/blueskit/siemens-s7-ppi-monitor/releases

## 使用方法:
  1) 在PC上可使用 USB-RS485转换器。 
  2) 在PLC和HMI通讯的RS485串口电缆上，跨接上述 RS485(仅用作读取，不做任何写操作)。 
  3) 运行本软件后，可显示串口上往返的各变量读写。 
  4) 因无法确定变量类型，全部按可能的数据类型解读并显示。 

## User Guide:
  1) Use a USB-RS485 converter on the PC.
  2) Tap the above-mentioned RS485 converter into the RS485 serial cable for PLC-HMI communication (read-only mode, no write operations performed).
  3) After running the software, it can display all variable read/write operations transmitted back and forth through the serial port.
  4) Since the variable types cannot be determined, all variables are interpreted and displayed according to all possible data types.
