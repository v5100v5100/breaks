# Address Bus

![6502_locator_addr](/BreakingNESWiki/imgstore/6502_locator_addr.jpg)

Although the 6502 communicates with the outside world on a 16-bit address bus, but because the processor is 8-bit in nature, the address bus is internally divided into two 8-bit halves: an upper (ADH) and a lower (ADL).

The internal ADH/ADL address bus connects to the external 16-bit bus (pins A0-A15) through registers ABH/ABL, which contain the last written value.

The address bus is unidirectional. It can only be controlled by the 6502.

Transistor circuit of the lower bits of the ABL (0-2):

![abl02_tran](/BreakingNESWiki/imgstore/abl02_tran.jpg)

(The schematic is the same for ABL1 and ABL2 bits)

The remaining ABL bits (3-7):

![abl37_tran](/BreakingNESWiki/imgstore/abl37_tran.jpg)

ABH bits:

![abh_tran](/BreakingNESWiki/imgstore/abh_tran.jpg)

- 0/ADL0, 0/ADL1, 0/ADL2: The lower 3 bits of the ADL bus can be forced to zero by commands when setting [interrupts vector](interrupts.md)
- ADL/ABL: Place the value of the internal ADL bus on the ABL register
- ADH/ABH: Place the ADH internal bus value on the ABH register