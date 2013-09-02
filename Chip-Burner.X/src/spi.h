/* 
 * File:   spi.h
 * Author: rudarobson
 *
 * Created on 12 de Agosto de 2013, 13:08
 */
#include "leonino.h"

#ifndef SPI_H
#define	SPI_H

#ifdef	__cplusplus
extern "C" {
#endif

#define SPI_TYPE_START	0
#define SPI_TYPE_PUSH	1
#define SPI_TYPE_PULL	2
#define SPI_TYPE_ERASE  3
#define SPI_TYPE_RUN    4

    /*
     * Serial Data Out (SDO) ? RC7/RX/DT/SDO
     * Serial Data In (SDI) ? RB0/AN12/INT0/FLT0/SDI/SDA
     * Serial Clock (SCK) ? RB1/AN10/INT1/SCK/SCL
     */

#define PIN_VCC		PORTBbits.RB3
#define PIN_VCC_ENABLE (TRISBbits.RB3 = 0)
#define PIN_VCC_DISABLE (TRISBbits.RB3 = 1)

#define PIN_RESET	PORTDbits.RD0
#define PIN_RESET_ENABLE (TRISDbits.RD0 = 0)
#define PIN_RESET_DISABLE (TRISDbits.RD0 = 1)

#define PIN_MOSI	PORTCbits.RC7
#define PIN_MOSI_ENABLE (TRISCbits.RC7 = 0)
#define PIN_MOSI_DISABLE (TRISCbits.RC7 = 1)

#define PIN_SCLK	PORTBbits.RB1
#define PIN_SCLK_ENABLE (TRISBbits.RB1 = 0)
#define PIN_SCLK_DISABLE (TRISBbits.RB1 = 1)

#define PIN_MISO	PORTBbits.RB0
#define PIN_MISO_ENABLE (TRISBbits.RB0 = 1)
#define PIN_MISO_DISABLE (TRISBbits.RB0 = 1)

    typedef struct SPI_ProgrammingDelays {
        unsigned char tWD_FLASH;
        unsigned char tWD_EEPROM;
        unsigned char tWD_ERASE;
        unsigned char tWD_FUSE;
    } SPI_ProgrammingDelays;

    typedef struct SPI_MMDelays {
        unsigned char tCLCL;
        unsigned char tSHSL;
        unsigned char tSLSH;
        unsigned char tOVSH;
        unsigned char tSLIV;
    } SPI_MMDelays;


    void SPI_CONFIGURE();
    void SPI_UNCONFIGURE();

    //inst is the instruction to execute on SPI interface
    std_byte spi_transcation(std_byte b);
    void SPI_Write(std_byte b3, std_byte b2, std_byte b1, std_byte b0, std_byte *out);

    //return 1 if succeeded or 0
    std_byte SPI_EnableProgramming();
    void SPI_ChipErase();
    /*
     * hl -> 0 means low byte, anything else means high byte
     * address -> full address to store, only lowest part is used
     */
    std_byte SPI_ReadProgramMemory(std_byte hl, std_int32 address);
    void SPI_LoadProgramMemory(std_byte data, std_byte hl, std_int32 address);
    void SPI_WriteProgramMemoryPage(std_int32 address);
    void SPI_Main(char *data, unsigned int received);

    extern SPI_ProgrammingDelays spi_delays;
    extern SPI_MMDelays spi_mmDelays;
#ifdef	__cplusplus
}
#endif

#endif	/* SPI_H */

