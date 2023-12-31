﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Base
{
    enum WINBASE
    {
        NOPARITY           =0,
        ODDPARITY          =1,
        EVENPARITY         =2,
        MARKPARITY         =3,
        SPACEPARITY        =4,

        ONESTOPBIT         =0,
        ONE5STOPBITS       =1,
        TWOSTOPBITS        =2
    }

    enum PLC_CPU_CODE
    {
        /// CPU TYPE--------------------------------------------------------------------
        // QnACPU
        CPU_Q2ACPU								=0x0011,	            // Q2A
        CPU_Q2AS1CPU							=0x0012,	            // Q2AS1
        CPU_Q3ACPU								=0x0013,	            // Q3A
        CPU_Q4ACPU								=0x0014,	            // Q4A
        // QCPU Q                
        CPU_Q02CPU								=0x0022,	            // Q02(H) Q
        CPU_Q06CPU								=0x0023,	            // Q06H   Q
        CPU_Q12CPU								=0x0024,	            // Q12H   Q
        CPU_Q25CPU								=0x0025,	            // Q25H   Q
        CPU_Q00JCPU								=0x0030,	            // Q00J	  Q
        CPU_Q00CPU								=0x0031,	            // Q00	  Q
        CPU_Q01CPU								=0x0032,	            // Q01	  Q
        CPU_Q12PHCPU							=0x0041,	            // Q12PHCPU Q
        CPU_Q25PHCPU							=0x0042,	            // Q25PHCPU Q
        CPU_Q12PRHCPU							=0x0043,	            // Q12PRHCPU Q
        CPU_Q25PRHCPU							=0x0044,	            // Q25PRHCPU Q
        CPU_Q25SSCPU							=0x0055,	            // Q25SS
        CPU_Q03UDCPU							=0x0070,	            // Q03UDCPU
        CPU_Q04UDHCPU							=0x0071,	            // Q04UDHCPU
        CPU_Q06UDHCPU							=0x0072,	            // Q06UDHCPU
        CPU_Q02UCPU								=0x0083,	            // Q02UCPU
        CPU_Q03UDECPU							=0x0090,	            // Q03UDECPU
        CPU_Q04UDEHCPU							=0x0091,	            // Q04UDEHCPU
        CPU_Q06UDEHCPU							=0x0092,	            // Q06UDEHCPU
        CPU_Q13UDHCPU							=0x0073,	            // Q13UDHCPU
        CPU_Q13UDEHCPU							=0x0093,	            // Q13UDEHCPU
        CPU_Q26UDHCPU							=0x0074,	            // Q26UDHCPU	
        CPU_Q26UDEHCPU							=0x0094,	            // Q26UDEHCPU
        CPU_Q02PHCPU							=0x0045,	            // Q02PH  Q
        CPU_Q06PHCPU							=0x0046,	            // Q06PH  Q
        CPU_Q00UJCPU							=0x0080,	            // Q00UJCPU
        CPU_Q00UCPU								=0x0081,	            // Q00UCPU
        CPU_Q01UCPU								=0x0082,	            // Q01UCPU
        CPU_Q10UDHCPU							=0x0075,	            // Q10UDHCPU
        CPU_Q20UDHCPU							=0x0076,	            // Q20UDHCPU
        CPU_Q10UDEHCPU							=0x0095,	            // Q10UDEHCPU
        CPU_Q20UDEHCPU							=0x0096,	            // Q20UDEHCPU
        CPU_Q50UDEHCPU							=0x0098,	            // Q50UDEHCPU
        CPU_Q100UDEHCPU							=0x009A,	            // Q100UDEHCPU
        CPU_Q03UDVCPU							=0x00D1,	            // Q03UDVCPU
        CPU_Q04UDVCPU							=0x00D2,	            // Q04UDVCPU
        CPU_Q06UDVCPU							=0x00D3,	            // Q06UDVCPU
        CPU_Q13UDVCPU							=0x00D4,	            // Q13UDVCPU
        CPU_Q26UDVCPU							=0x00D5,	            // Q26UDVCPU         
        // ACPU                             
        CPU_A0J2HCPU							=0x0102,	            // A0J2H
        CPU_A1FXCPU								=0x0103,	            // A1FX
        CPU_A1SCPU								=0x0104,	            // A1S,A1SJ
        CPU_A1SHCPU								=0x0105,	            // A1SH,A1SJH
        CPU_A1NCPU								=0x0106,	            // A1(N)
        CPU_A2CCPU								=0x0107,	            // A2C,A2CJ
        CPU_A2NCPU								=0x0108,	            // A2(N),A2S
        CPU_A2SHCPU								=0x0109,	            // A2SH
        CPU_A3NCPU								=0x010A,	            // A3(N)
        CPU_A2ACPU								=0x010C,	            // A2A
        CPU_A3ACPU								=0x010D,	            // A3A
        CPU_A2UCPU								=0x010E,	            // A2U,A2US
        CPU_A2USHS1CPU							=0x010F,	            // A2USHS1
        CPU_A3UCPU								=0x0110,	            // A3U
        CPU_A4UCPU								=0x0111,	            // A4U
        // QCPU A        
        CPU_Q02CPU_A							=0x0141,	            // Q02(H)
        CPU_Q06CPU_A							=0x0142,	            // Q06H
        // LCPU                         
        CPU_L02CPU								=0x00A1,	            // L02CPU
        CPU_L26CPUBT							=0x00A2,	            // L26CPU-BT
        CPU_L02SCPU								=0x00A3,	            // L02SCPU
        CPU_L26CPU								=0x00A4,	            // L26CPU
        CPU_L06CPU								=0x00A5,	            // L06CPU
        // C Controller                     
        CPU_Q12DC_V								=0x0058,	            // Q12DCCPU-V
        CPU_Q24DHC_V							=0x0059,	            // Q24DHCCPU-V
        CPU_Q24DHC_VG							=0x005C,	            // Q24DHCCPU-VG
        CPU_Q26DHC_LS							=0x005D,	            // Q26DHCCPU-LS
        // Q MOTION 	              
        CPU_Q172CPU								=0x621,		            // Q172CPU
        CPU_Q173CPU								=0x622,		            // Q173CPU
        CPU_Q172HCPU							=0x621,	                // Q172HCPU
        CPU_Q173HCPU							=0x622,	                // Q173HCPU
        CPU_Q172DCPU							=0x625,	                // Q172DCPU
        CPU_Q173DCPU							=0x626,	                // Q173DCPU
        CPU_Q172DSCPU							=0x62A,	                // Q172DSCPU
        CPU_Q173DSCPU							=0x62B,	                // Q173DSCPU
        // QSCPU               
        CPU_QS001CPU							=0x006,	                // QS001
        // FXCPU                    
        CPU_FX0CPU								=0x020,	                // FX0/FX0S
        CPU_FX0NCPU								=0x0202,	            // FX0N
        CPU_FX1CPU								=0x0203,	            // FX1
        CPU_FX2CPU								=0x0204,	            // FX2/FX2C
        CPU_FX2NCPU								=0x0205,	            // FX2N/FX2NC
        CPU_FX1SCPU								=0x0206,	            // FX1S
        CPU_FX1NCPU								=0x0207,	            // FX1N/FX1NC
        CPU_FX3UCCPU							=0x0208,	            // FX3U/FX3UC 
        CPU_FX3GCPU								=0x0209,	            // FX3G/FX3GC
        // BOARD              
        CPU_BOARD								=0x0401,	            // NETWORK BOARD
        // MOTION 	                       
        CPU_A171SHCPU							=0x0601,	            // A171SH
        CPU_A172SHCPU							=0x0602,	            // A172SH
        CPU_A273UHCPU							=0x0603,	            // A273UH
        CPU_A173UHCPU							=0x0604,	            // A173UH
        // GOT                                 
        CPU_A900GOT								=0x0701,	            // A900GOT                                
        // iQ-R CPU                         
        CPU_R04CPU								=0x1001,	            // R04CPU
        CPU_R08CPU								=0x1002,	            // R08CPU
        CPU_R16CPU								=0x1003,	            // R16CPU
        CPU_R32CPU								=0x1004,	            // R32CPU
        CPU_R120CPU								=0x1005,	            // R120CPU
        CPU_R00CPU								=0x1201,	            // R00CPU
        CPU_R01CPU								=0x1202,	            // R01CPU
        CPU_R02CPU								=0x1203,	            // R02CPU                             
        // iQ-R Motion                    
        CPU_R16MTCPU							=0x1011,	            // R16MTCPU.
        CPU_R32MTCPU							=0x1012,	            // R32MTCPU.                      
        // iQ-R PROCESS CPU                
        CPU_R08PCPU								=0x1102,	            // R08PCPU
        CPU_R16PCPU								=0x1103,	            // R16PCPU
        CPU_R32PCPU								=0x1104,	            
        // R32PCPU
        CPU_R120PCPU							=0x1105,	            // R120PCPU                          
        // iQ-R SAFE CPU                       
        CPU_R08SFCPU							=0x1122	,	            // R08SFCPU
        CPU_R16SFCPU							=0x1123	,	            // R16SFCPU
        CPU_R32SFCPU							=0x1124	,	            // R32SFCPU
        CPU_R120SFCPU							=0x1125	,	            // R120SFCPU                               
        // iQ-R EN CPU            
        CPU_R04ENCPU							=0x1008,	            // R04ENCPU
        CPU_R08ENCPU							=0x1009,	            // R08ENCPU
        CPU_R16ENCPU							=0x100A,	            // R32ENCPU
        CPU_R32ENCPU							=0x100B,	            // R64ENCPU
        CPU_R120ENCPU							=0x100C,	            // R120ENCPU                         
        // iQ-R CCONTROLLER                 
        CPU_R12CCPU_V							=0x1021,	            // R12CCPU-V                        
        // iQ-R PSF CPU                 
        CPU_R08PSFCPU							=0x1111,	            // R08PSFCPU
        CPU_R16PSFCPU							=0x1112,	            // R16PSFCPU
        CPU_R32PSFCPU							=0x1113,	            // R32PSFCPU
        CPU_R120PSFCPU							=0x1114,	            // R120PSFCPU                           
        // iQ-F CPU                     
        CPU_FX5UCPU								=0x0210,	            // FX5U CPU                    
        // PORT----------------------------------------------------------------------------------                              
        PORT_1								    =0x01,	                // CommunicationPort1
        PORT_2								    =0x02,	                // CommunicationPort2
        PORT_3								    =0x03,	                // CommunicationPort3
        PORT_4								    =0x04,	                // CommunicationPort4
        PORT_5								    =0x05,	                // CommunicationPort5
        PORT_6								    =0x06,	                // CommunicationPort6
        PORT_7								    =0x07,	                // CommunicationPort7
        PORT_8								    =0x08,	                // CommunicationPort8
        PORT_9								    =0x09,	                // CommunicationPort9
        PORT_10								    =0x0A,			        // CommunicationPort10                                           
        // BAUDRATE-------------------------------------------------------------------------------                         
        BAUDRATE_300						    =300   ,                // 300bps
        BAUDRATE_600							=600   ,  	            // 600bps
        BAUDRATE_1200							=1200  , 	            // 1200bps
        BAUDRATE_2400							=2400  , 	            // 2400bps
        BAUDRATE_4800							=4800  , 	            // 4800bps
        BAUDRATE_9600							=9600  , 	            // 9600bps
        BAUDRATE_19200							=19200 , 	            // 19200bps
        BAUDRATE_38400							=38400 , 	            // 38400bps
        BAUDRATE_57600							=57600 , 	            // 57600bps
        BAUDRATE_115200						    =115200,                // 115200bps                               
        // DATA BIT-------------------------------------------------------------------------------                          
        DATABIT_7								=7,	                    // DATA BIT 7
        DATABIT_8								=8,	                    // DATA BIT 8                                              
        // PARITY---------------------------------------------------------------------------------                        
        NO_PARRITY                              = WINBASE.NOPARITY  , 	// NO PARITY
        ODD_PARITY                              = WINBASE.ODDPARITY , 	// ODD PARITY
        EVEN_PARITY								= WINBASE.EVENPARITY,   // EVEN PARITY                                                          
        // STOP BITS------------------------------------------------------------------------------       
        STOPBIT_ONE								= WINBASE.ONESTOPBIT ,  // 1 STOP BIT
        STOPBIT_TWO								= WINBASE.TWOSTOPBITS,  // 2 STOP BIT                                              
        // SERIAL CONTROL-------------------------------------------------------------------------                
        TRC_DTR									=0x01,		            // DTR
        TRC_RTS									=0x02,		            // RTS
        TRC_DTR_AND_RTS							=0x07,		            // DTR and RTS 
        TRC_DTR_OR_RTS							=0x08,	                // DTR or RTS                                 
        // SUM CHECK------------------------------------------------------------------------------                   
        SUM_CHECK								=1,		                // Sum Check
        NO_SUM_CHECK							=0,		                // No Sum Check                                                            
        //PACKET TYPE-----------------------------------------------------------------------------                                          
        PACKET_ASCII							=0x02,	                //PACKET TYPE ASCII
        PACKET_BINARY							=0x03,	                //PACKET TYPE BINARY                           
        //DELIMITER------------------------------------------------------------------------------- 
        CRLF_NONE								=0,	                    // CR/LF None
        CRLF_CR									=1,		                // CR ONLY
        CRLF_CRLF								=2,	                    // CR/LF                         
        //CONNECT WAY-----------------------------------------------------------------------------  
        TEL_AUTO_CONNECT						=0x00,	                // AUTO LINE CONNECT
        TEL_AUTO_CALLBACK						=0x01,	                // AUTO LINE CONNECT(CALLBACK FIXATION)
        TEL_AUTO_CALLBACK_NUMBER				=0x02,	                // AUTO LINE CONNECT(CALLBACK NUMBER SPECIFICATION)
        TEL_CALLBACK							=0x03,	                // CALLBACK CONNECT(FIXATION)
        TEL_CALLBACK_NUMBER					    =0x04,                  // CALLBACK CONNECT(NUMBER SPECIFICATION)
        TEL_CALLBACK_REQUEST					=0x05,	                // CALLBACK REQUEST(FIXATION)
        TEL_CALLBACK_REQUEST_NUMBER			    =0x06,	                // CALLBACK REQUEST(NUMBER SPECIFICATION)
        TEL_CALLBACK_WAIT						=0x07,	                // CALLBACK RECEPTION WAITING                                  
        //LINE TYPE-------------------------------------------------------------------------------                   
        LINETYPE_PULSE							=0x00,	                // PULSE
        LINETYPE_TONE							=0x01,	                // TONE
        LINETYPE_ISDN							=0x02,	                // ISDN
        //GOT TRANSPARENT PC IF-------------------------------------------------------------------
        GOT_PCIF_USB			                =1,		                // USB
        GOT_PCIF_SERIAL			                =2,		                // SERIAL
        GOT_PCIF_ETHERNET		                =3,		                // ETHERNET
        //GOT TRANSPARENT PLC IF------------------------------------------------------------------
        GOT_PLCIF_SERIAL_QCPUQ		            =1,	                    // SERIAL-QCPU Q
        GOT_PLCIF_SERIAL_QCPUA		            =2,	                    // SERIAL-QCPU A
        GOT_PLCIF_SERIAL_QNACPU	                =3,	                    // SERIAL-QnACPU
        GOT_PLCIF_SERIAL_ACPU		            =4,	                    // SERIAL-ACPU
        GOT_PLCIF_SERIAL_FXCPU		            =5,	                    // SERIAL-FXCPU
        GOT_PLCIF_SERIAL_LCPU		            =6,	                    // SERIAL-LCPU
        GOT_PLCIF_SERIAL_QJ71C24	            =30,	                // SERIAL-QJ71C24
        GOT_PLCIF_SERIAL_LJ71C24	            =31,	                // SERIAL-LJ71C24
        GOT_PLCIF_ETHERNET_QJ71E71	            =50,	                // ETHERNET-QJ71E71
        GOT_PLCIF_ETHERNET_CCIEFADP	            =60,	                // ETHERNET-CC IE Field adapter
        GOT_PLCIF_ETHERNET_QCPU		            =70,	                // ETHERNET-QCPU
        GOT_PLCIF_ETHERNET_LCPU		            =71,	                // ETHERNET-LCPU
        GOT_PLCIF_BUS				            =90,	                // BUS
        //ACTPROGTYPE UNITTYPE--------------------------------------------------------------------
        UNIT_QNCPU				                =0x13,	                // SERIAL(RS232C)-QCPU Q
        UNIT_FXCPU				                =0x0F,	                // SERIAL(RS232C)-FXCPU
        UNIT_LNCPU				                =0x50,	                // SERIAL(RS232C)-LCPU
        UNIT_QNMOTION			                =0x1C,	                // SERIAL(RS232C)-QMOTION
        UNIT_QJ71C24			                =0x19,	                // SERIAL(C24)-QCPU
        UNIT_FX485BD			                =0x24,	                // SERIAL(FX485BD)-FXCPU
        UNIT_LJ71C24			                =0x54,	                // SERIAL(C24)-LCPU
        UNIT_QJ71E71			                =0x1A,	                // Ethernet(QJ71E71)
        UNIT_FXENET			                    =0x26,	                // Ethernet(FXENET)
        UNIT_FXENET_ADP		                    =0x27,	                // Ethernet(FX1N-ENET-ADP)
        UNIT_QNETHER			                =0x2C,	                // Ethernet(QCPU) IP
        UNIT_QNETHER_DIRECT	                    =0x2D,	                // Ethernet(QCPU) DIRECT
        UNIT_LNETHER			                =0x52,	                // Ethernet(LCPU) IP
        UNIT_LNETHER_DIRECT	                    =0x53,	                // Ethernet(LCPU) DIRECT
        UNIT_NZ2GF_ETB			                =0x59,	                // NZ2GF-ETB
        UNIT_NZ2GF_ETB_DIRECT	                =0x5A,	                // NZ2GF-ETB DIRECT
        UNIT_QNUSB				                =0x16,	                // USB-QCPU
        UNIT_LNUSB				                =0x51,                  // USB-LCPU
        UNIT_QNMOTIONUSB		                =0x1D,                  // USB-QMOTION
        UNIT_G4QNCPU			                =0x1B,	                // G4-QCPU
        UNIT_CCLINKBOARD		                =0x0C,	                // CC-Link Board 
        UNIT_MNETHBOARD		                    =0x1E,                  // MELSECNET/H Board 
        UNIT_MNETGBOARD		                    =0x2B,	                // CC-Link IE Control Board 
        UNIT_CCIEFBOARD		                    =0x2F,	                // CC-Link IE Field Board 
        UNIT_SIMULATOR			                =0x0B,	                // GX Simulator
        UNIT_SIMULATOR2		                    =0x30,	                // GX Simulator2
        //GOT UNITTYPE----------------------------------------------------------------------------
        UNIT_QBF                                =0x1F,	                // QBF
        UNIT_QSS				                =0x20,	                // Qn SoftLogic
        UNIT_A900GOT			                =0x21,	                // GOT
        UNIT_GOT_QJ71E71		                =0x40,	                // GOT Transparent QJ71E71
        UNIT_GOT_QNETHER		                =0x41,	                // GOT Transparent Ethernet(QCPU)
        UNIT_GOT_LNETHER		                =0x55,	                // GOT Transparent Ethernet(LCPU)
        UNIT_GOT_NZ2GF_ETB		                =0x5B,	                // GOT Transparent NZ2GF-ETB
        UNIT_GOTETHER_QNCPU	                    =0x56,	                // GOT Transparent ETHERNET-QCPU
        UNIT_GOTETHER_QBUS		                =0x58,	                // GOT Transparent ETHERNET-QBUS
        UNIT_GOTETHER_LNCPU	                    =0x57,	                // GOT Transparent ETHERNET-LCPU
        UNIT_FXETHER			                =0x4A,	                // EthernetADP-FXCPU
        UNIT_FXETHER_DIRECT	                    =0x4B,	                // EthernetADP-FXCPU(DIRECT)
        UNIT_GOTETHER_FXCPU	                    =0x60,	                // GOT Transparent Ethernet(FXCPU)
        UNIT_GOT_FXETHER		                =0x61,	                // GOT Transparent FX3U-ENET-ADP
        UNIT_GOT_FXENET		                    =0x62,	                // GOT FX3U-ENET(-L)
        UNIT_RJ71C24			                =0x1000,                // SERIAL(C24)-RCPU
        UNIT_RJ71EN71			                =0x1001,                // Ethernet(RJ71EN71)
        UNIT_RETHER			                    =0x1002,                // Ethernet(RCPU) IP
        UNIT_RETHER_DIRECT		                =0x1003,                // Ethernet(RCPU) DIRECT
        UNIT_RUSB				                =0x1004,                // USB-RCPU
        UNIT_RJ71EN71_DIRECT	                =0x1005,                // Ethernet(RJ71EN71) DIRECT
        UNIT_GOT_RJ71EN71		                =0x1051,                // GOT Transparent RJ71EN71
        UNIT_GOT_RETHER		                    =0x2000,                // SERIAL(RS232C)-FX5CPU
        UNIT_FXVETHER			                =0x2001,                // Ethernet(FX5CPU) IP
        UNIT_FXVETHER_DIRECT	                =0x2002,                // Ethernet(FX5CPU) DIRECT
        UNIT_SIMULATOR3		                    =0x31,                  // GX Simulator3
        UNIT_GOT_FXVCPU		                    =0x2005,                // GOT Transparent SERIAL(FX5U)
        UNIT_GOTETHER_FXVCPU	                =0x2006,                // GOT Transparent Ethernet-FX5U
        UNIT_GOT_FXVETHER		                =0x2007,                // GOT Transparent ETHERNET(FX5U)
        UNIT_LJ71E71			                =0x5C,                  // Ethernet(LJ71E71)
        UNIT_GOT_LJ71E71		                =0x5D,                  // GOT Transparent LJ71E71
        UNIT_GOTETHER_QN_ETHER	                =0x6F,                  // GOT Transparent(Ether-GOT-Ether-QnCPU)
        //ACTPROGTYPE PROTOCOLTYPE----------------------------------------------------------------
        PROTOCOL_SERIAL		                    =0x04,	                // Protocol Serial
        PROTOCOL_USB			                =0x0D,	                // Protocol USB
        PROTOCOL_TCPIP			                =0x05,	                // Protocol TCP/IP 
        PROTOCOL_UDPIP			                =0x08,	                // Protocol UDP/IP 
        PROTOCOL_MNETH			                =0x0F,	                // Protocol MELSECNET/H
        PROTOCOL_MNETG			                =0x14,	                // Protocol CC IE Control Board
        PROTOCOL_CCIEF			                =0x15,	                // Protocol CC IE Field Board
        PROTOCOL_CCLINK		                    =0x07,	                // Protocol CC-LINK Board
        PROTOCOL_SERIALMODEM	                =0x0E,	                // Protocol MODEM
        PROTOCOL_TEL			                =0x0A,	                // Protocol TEL
        PROTOCOL_QBF			                =0x10,	                // Protocol QBF
        PROTOCOL_QSS			                =0x11,	                // Protocol QSS
        PROTOCOL_USBGOT		                    =0x13,	                // Protocol GOT TRANSPARENT USB
        PROTOCOL_SHAREDMEMORY	                =0x06,	                // Protocol Simulator
        //ACTPROGTYPE INVERTER PROTOCOLTYPE-------------------------------------------------------
        COMM_RS232C		                        =0x00,	                // Serial INVERTER
        COMM_USB				                =0x01,	                // USB INVERTER
        //ACTPROGTYPE ROBOTCONTROLLER PROTOCOLTYPE------------------------------------------------
        RC_PROTOCOL_SERIAL		                =0x01,	                // Serial Robot Controller
        RC_PROTOCOL_USB		                    =0x04,	                // USB Robot Controller                                                                                                                                                                                      0x04	// USB Robot Controller
        RC_PROTOCOL_TCPIP		                =0x02	                // TCP/IP Robot Controller

    }
}
