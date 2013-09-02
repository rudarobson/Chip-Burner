/* 
 * File:   main.C
 * Author: rudarobson
 *
 * Created on 26 de Agosto de 2013, 01:42
 */
#include <leonino.h>
#include "spi.h"

typedef struct cmd{
    unsigned char cmd;
    unsigned int address;
}CMD;

void read() {
    char *data[USB_MAX_READ_SIZE];
    usb_read(data,USB_MAX_READ_SIZE);
    CMD *cmd = data;
    
    switch (cmd->cmd) {
        case SPI_TYPE_START:
            if (SPI_EnableProgramming())
                usb_write_byte(0x53);
            else
                usb_write_byte(0x35);
            usb_send();
            break;
        case SPI_TYPE_ERASE:
            SPI_ChipErase();
            usb_write_byte('O');
            usb_write_byte('K');
            usb_send();
            break;
        case SPI_TYPE_PUSH:
            if (1) {//just to bypass the compiler and declare local scope variables
                std_byte count;
                std_int32 aux;
                std_int32 address = 0;
                std_byte i;
                std_byte high = 0;

                for (i = 0; i < 4; i++) {
                    aux = (data[1 + i]) & 0xFF;
                    address |= (aux << (8 * (3 - i)));
                }

                for (count = 0; count < data[5]; count++) {
                    SPI_LoadProgramMemory(data[6 + count], high, address + ((std_int32) count / 2));

                    if (high == 0)
                        high = 1;
                    else
                        high = 0;
                }
                SPI_WriteProgramMemoryPage(address);

                usb_write_byte('O');
                usb_write_byte('K');
            }
            usb_send();
            break;
        case SPI_TYPE_PULL:
            if (1) {
                std_byte count;
                std_int32 aux;
                std_int32 address = 0;
                std_byte i;
                std_byte high = 0;
                std_byte data_read;

                for (i = 0; i < 4; i++) {
                    aux = (data[1 + i]) & 0xFF;
                    address |= (aux << (8 * (3 - i)));
                }

                for (count = 0; count < data[5]; count++) {
                    data_read = SPI_ReadProgramMemory(high, address + ((std_int32) count / 2));

                    if (high == 0)
                        high = 1;
                    else
                        high = 0;

                    usb_write_byte(data_read);
                }
            }
            usb_send();
            break;
        case SPI_TYPE_RUN:
            SPI_UNCONFIGURE();
            if (data[1] == 1) {
                PIN_VCC_ENABLE;
                PIN_VCC = 1;
            } else {
                PIN_VCC = 0;
                PIN_VCC_DISABLE;
            }
            break;
    }
}

int main() {
    usb_configure_handler(read);

    for (;;) {
        usb_handle();
    }

}

