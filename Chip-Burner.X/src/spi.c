#include <htc.h>
#include "spi.h"
#include "leonino.h"

SPI_ProgrammingDelays spi_delays;
SPI_MMDelays spi_mmDelays;

void SPI_CONFIGURE() {
    ADCON0 = 0x3C; // Disabling the ADC module which is multiplexed with SPI pins
    ADCON1 = 0x0F; // Disabling the ADC module which is multiplexed with SPI pins
    CMCON = 0x07; // Disabling the COMPARATOR module which is multiplexed with SPI pins
    SPPCON = 0x00; // Disabling the SERIAL PERIPHERAL CONTROL module which is multiplexed with SPI
    CVREN = 0x80;


    PIN_VCC_ENABLE;
    PIN_RESET_ENABLE;
    PIN_MISO_ENABLE;
    PIN_SCLK_ENABLE;
    PIN_MOSI_ENABLE;

    PIN_VCC = 0;
    PIN_RESET = 1;

}

void SPI_UNCONFIGURE() {
    PIN_RESET = 1;
    PIN_VCC = 0;

    PIN_VCC_DISABLE;
    PIN_RESET_DISABLE;
    PIN_MISO_DISABLE;
    PIN_SCLK_DISABLE;
    PIN_MOSI_DISABLE;
}

std_byte spi_transcation(std_byte b) {
    //	std_byte trash_protocol = SSPBUF;
    //	PIR1 &= 0xf7;
    //	SSPBUF = b;
    //	while (!SSPSTATbits.BF); // wait until the all bits received
    //
    //	PIR1 &= 0xf7;
    //	return SSPBUF;


    std_byte data_read = 0;
    char i;
    for (i = 8; i > 0; i--) {
        PIN_MOSI = ((b >> (i - 1)) & 0x01);
        _delay((INST_FREQ_us) * 15); //15 us

        PIN_SCLK = 1;
        _delay((INST_FREQ_us) * 75); //15 us

        data_read = (data_read << 1) | (PIN_MISO);
        PIN_SCLK = 0;
        _delay((INST_FREQ_us) * 75); //15 us
        _delay((INST_FREQ_us) * 15); //15 us
    }
    return data_read;
}

void SPI_Write(std_byte b3, std_byte b2, std_byte b1, std_byte b0, std_byte *out) {
    b3 = spi_transcation(b3);
    b2 = spi_transcation(b2);
    b1 = spi_transcation(b1);
    b0 = spi_transcation(b0);

    if (out != 0) {
        out[0] = b3; //trash
        out[1] = b2;
        out[2] = b1;
        out[3] = b0;
    }
}

std_byte SPI_EnableProgramming() {
    std_byte out[4];
    PIN_RESET = 1;
    PIN_VCC = 1;
    delay_ms(1); //at least 2 cycles of programming chip
    PIN_RESET = 0;
    delay_ms(22); //delay at least 20 ms

    SPI_Write(0xAC, 0x53, 0x00, 0x00, out);

    if (out[2] == 0x53)
        return 1;
    return 0;
}

void SPI_ChipErase() {
    SPI_Write(0xAC, 0x80, 0x00, 0x00, 0);
    delay_ms(spi_delays.tWD_ERASE);
}

std_byte SPI_ReadProgramMemory(std_byte hl, std_int32 address) {
    std_byte buf[4];
    SPI_Write((0x20 | ((hl & 0x01) << 3)),
            ((address & 0x300) >> 8), //get 9th and 8th bit
            (address & 0xFF),
            0x00,
            buf); //anything, just to get output
    return (std_byte) buf[3];
}

void SPI_LoadProgramMemory(std_byte data, std_byte hl, std_int32 address) {
    SPI_Write((0x40 | ((hl & 0x01) << 3)),
            0x00,
            (address & 0x0F), //address low part
            data,
            0);
}

void SPI_WriteProgramMemoryPage(std_int32 address) {
    SPI_Write(0x4C,
            ((address & 0x300) >> 8),
            (address & 0xF0),
            0x00,
            0);
    delay_ms(spi_delays.tWD_FLASH);
}

void SPI_ExitProgramming() {
    SPI_UNCONFIGURE();
}

unsigned char toggled = 0;

void SPI_Main(char *data, unsigned int received) {
   
}
