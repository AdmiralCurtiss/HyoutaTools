/**
 * \file ccitt.c
 * Functions and types for CRC checks.
 *
 * Generated on Sun Oct 30 02:18:53 2016,
 * by pycrc v0.9, https://pycrc.org
 * using the configuration:
 *    Width         = 16
 *    Poly          = 0x1021
 *    Xor_In        = 0xffff
 *    ReflectIn     = False
 *    Xor_Out       = 0x0000
 *    ReflectOut    = False
 *    Algorithm     = bit-by-bit-fast
 *****************************************************************************/
#include "ccitt.h"     /* include the header file generated with pycrc */
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

/**
 * Update the crc value with new data.
 *
 * \param crc      The current crc value.
 * \param data     Pointer to a buffer of \a data_len bytes.
 * \param data_len Number of bytes in the \a data buffer.
 * \return         The updated crc value.
 *****************************************************************************/
crc_t crc_update(crc_t crc, const void *data, size_t data_len)
{
    const unsigned char *d = (const unsigned char *)data;
    unsigned int i;
    bool bit;
    unsigned char c;

    while (data_len--) {
        c = *d++;
        for (i = 0x80; i > 0; i >>= 1) {
            bit = crc & 0x8000;
            if (c & i) {
                bit = !bit;
            }
            crc <<= 1;
            if (bit) {
                crc ^= 0x1021;
            }
        }
        crc &= 0xffff;
    }
    return crc & 0xffff;
}


