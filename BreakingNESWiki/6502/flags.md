# Флаги

![6502_locator_flags](/BreakingNESWiki/imgstore/6502/6502_locator_flags.jpg)

Флаги (разряды регистра P) находятся в "размазанном" виде, как несколько схем верхней части процессора.

Управление флагами производится в схеме [управления флагами](flags_control.md).

Флаг B рассматривается отдельно, в разделе посвященном [обработке прерываний](interrupts.md). Топологически он также находится в другой части процессора.

## Флаг C

![flag_c_tran](/BreakingNESWiki/imgstore/6502/flag_c_tran.jpg)

- IR5/C: Изменить значение флага в соответствии с разрядом IR5 (применяется во время выполнения инструкций `SEC` и `CLC`)
- ACR/C: Изменить значение флага в соответствии со значением ACR
- DB/C: Изменить значение флага в соответствии с разрядом DB0
- /IR5: Значение разряда IR5, в инвертированном виде
- /DB0: Входное значение с шины DB, в инвертированном виде
- ACR: Результат переноса с АЛУ
- /ACR: Результат переноса с АЛУ, в инвертированном виде. Также используется в [диспатчере](dispatch.md)
- /C_OUT: Выходное значение флага C, в инвертированном виде

## Флаг D

![flag_d_tran](/BreakingNESWiki/imgstore/6502/flag_d_tran.jpg)

- IR5/D: Изменить значение флага в соответствии с разрядом IR5 (применяется во время выполнения инструкций `SED` и `CLD`)
- DB/P: Общий контрольный сигнал, поместить значение шины DB на регистр флагов P
- /IR5: Значение разряда IR5, в инвертированном виде
- /DB3: Входное значение с шины DB, в инвертированном виде
- /D_OUT: Выходное значение флага D, в инвертированном виде

## Флаг I

![flag_i_tran](/BreakingNESWiki/imgstore/6502/flag_i_tran.jpg)

- IR5/I: Изменить значение флага в соответствии с разрядом IR5 (применяется во время выполнения инструкций `SEI` и `CLI`)
- DB/P: Общий контрольный сигнал, поместить значение шины DB на регистр флагов P
- /IR5: Значение разряда IR5, в инвертированном виде
- /DB2: Входное значение с шины DB, в инвертированном виде
- /I_OUT: Выходное значение флага I, в инвертированном виде. Данный сигнал уходит в два места: в схему обработки прерываний и в схему для обмена значений регистра флагов с шиной DB (ниже).

Сигнал `/I_OUT` дополнительно модифицируется сигналом `BRK6E` в схеме флага B:

![intr_b_flag_tran](/BreakingNESWiki/imgstore/intr_b_flag_tran.jpg)

## Флаг N

![flag_n_tran](/BreakingNESWiki/imgstore/6502/flag_n_tran.jpg)

- DB/N: Изменить значение флага в соответствии с разрядом DB7
- /DB7: Входное значение с шины DB, в инвертированном виде
- /N_OUT: Выходное значение флага N, в инвертированном виде

## Флаг V

![flag_v_tran](/BreakingNESWiki/imgstore/6502/flag_v_tran.jpg)

- 0/V: Очистить флаг V (применяется во время выполнения инструкций `CLV`)
- 1/V: Установить флаг V. Принудительная установка флага производится с помощью контакта `SO`.
- AVR/V: Изменить значение флага в соответствии со значением AVR
- DB/V: Изменить значение флага в соответствии с разрядом DB6
- AVR: Результат переполнения с АЛУ
- SO: Входное значение с контакта `SO`
- /DB6: Входное значение с шины DB, в инвертированном виде
- /V_OUT: Выходное значение флага V, в инвертированном виде

## Флаг Z

![flag_z_tran](/BreakingNESWiki/imgstore/6502/flag_z_tran.jpg)

- DBZ/Z: Изменить значение флага в соответствии со значением /DBZ
- DB/P: Общий контрольный сигнал, поместить значение шины DB на регистр флагов P
- /DBZ: Контрольный сигнал со схемы обмена флагов с шиной DB (проверка что все разряды шины DB равны 0)
- /DB1: Входное значение с шины DB, в инвертированном виде
- /Z_OUT: Выходное значение флага Z, в инвертированном виде

## Флаги и шина DB

![flags_io_tran](/BreakingNESWiki/imgstore/6502/flags_io_tran.jpg)

- С_OUT: Значение флага C в прямом виде, используется в [схеме управления АЛУ](alu_control.md) (в схеме для формирования сигнала `ADD/SB7`)
- D_OUT: Значение флага D в прямом виде, используется в схеме управления АЛУ (для формирования сигналов BCD коррекции DAA/DSA)
- P/DB: Поместить значение регистра флагов P на шину DB
- /DB0-7: Значение разрядов шины DB, в инвертированном виде. Подается на вход соответствующих разрядов регистра P.
- /DBZ: Проверка что все разряды шины DB равны 0 (ну то есть проверка значения на 0). Используется флагом Z.

Соответствие разрядов шины DB и регистра флагов P:

|Разряд DB|Флаг|
|---|---|
|0|C|
|1|Z|
|2|I|
|3|D|
|4|B|
|5|-|
|6|V|
|7|N|

Флаг 5 не используется. При сохранении регистра P на шину DB разряд DB5 не изменяется (не подключен). Однако значение разряда DB5 проверяется контрольным сигналом `/DBZ` (для сравнения значения на шине DB с нулем).

## Логическая схема

![flags_logisim](/BreakingNESWiki/imgstore/logisim/flags_logisim.jpg)

## Оптимизированная логическая схема

![15_flags_logisim](/BreakingNESWiki/imgstore/6502/ttlworks/15_flags_logisim.png)
