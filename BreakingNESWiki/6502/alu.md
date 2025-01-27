# АЛУ

![6502_locator_alu](/BreakingNESWiki/imgstore/6502/6502_locator_alu.jpg)

Показать всю схему АЛУ целиком не представляется возможным, поэтому распилим её на составные части и рассмотрим каждую по отдельности.

![alu_preview](/BreakingNESWiki/imgstore/6502/alu_preview.jpg)

АЛУ состоит из следующих компонентов:
- Входные схемы для загрузки защелок AI/BI
- Основная вычислительная часть (Operations)
- Схема быстрого вычисления переноса для BCD 
- Промежуточный результат (защёлка ADD)
- Схема BCD коррекции
- Аккумулятор (AC)

Вообще говоря, АЛУ представляет собой месиво из транзисторов и проводов, но его работа не очень сложная, в чем вы можете убедиться далее.

Большая схема:

![alu_logisim](/BreakingNESWiki/imgstore/6502/alu_logisim.jpg)

## Защёлки AI/BI

Входные схемы состоят из 8 одинаковых кусков, которые предназначены для загрузки входных значений на защёлки AI и BI:

![alu_input_tran](/BreakingNESWiki/imgstore/6502/alu_input_tran.jpg)

(На картинке показана схема для разряда 0, остальные аналогично)

Контрольные сигналы:
- DB/ADD: Загрузить прямое значение с шины DB в защёлку BI
- NDB/ADD: Загрузить инверсное значение с шины DB в защёлку BI
- ADL/ADD: Загрузить значение с шины ADL в защёлку BI
- SB/ADD: Загрузить значение с шины SB в защёлку AI
- 0/ADD: Записать 0 в защёлку AI

## Вычислительная часть

АЛУ использует инвертированную цепочку переносов, поэтому схемы четных и нечетных разрядов чередуются.

Разряд 0 немного отличается от остальных четных разрядов, так как на него приходит входной перенос (`/ACIN`) и отсутствует вход `SRS`.

Схема для разряда 0:

![alu_bit0_tran](/BreakingNESWiki/imgstore/6502/alu_bit0_tran.jpg)

Схемы для разрядов 2, 4, 6:

![alu_bit_even_tran](/BreakingNESWiki/imgstore/6502/alu_bit_even_tran.jpg)

(Показана схема для разряда 2, остальные аналогично)

Схемы для разрядов 1, 3, 5, 7:

![alu_bit_odd_tran](/BreakingNESWiki/imgstore/6502/alu_bit_odd_tran.jpg)

(Показана схема для разряда 1, остальные аналогично)

Оптимизированная логическая схема:

|Even|Odd|
|---|---|
|![1_alu_even_bit_logisim](/BreakingNESWiki/imgstore/6502/ttlworks/1_alu_even_bit_logisim.png)|![2_alu_odd_bit_logisim](/BreakingNESWiki/imgstore/6502/ttlworks/2_alu_odd_bit_logisim.png)|

Анатомически левая часть занимается логическими операциями, в правой части находится сумматор (Full Adder), а по середине - цепочка переноса.

Контрольные сигналы для операций АЛУ:
- ORS: Операция логического ИЛИ (AI | BI)
- ANDS: Операция логического И (AI & BI)
- EORS: Операция логического дополнения XOR (AI ^ BI)
- SRS: Сдвиг вправо. Для этого результат текущей операции `nand` сохраняется как результат предыдущего разряда.  
- SUMS: Суммирование (AI + BI)

Обозначения на схемах:
- nand: промежуточный результат выполнения операции NAND для выбранного разряда
- and: промежуточный результат выполнения операции AND для выбранного разряда (получается инверсией `nand`)
- nor: промежуточный результат выполнения операции NOR для выбранного разряда
- xor: промежуточный результат выполнения операции EOR для выбранного разряда
- nxor: промежуточный результат выполнения операции ENOR для выбранного разряда
- carry: результаты переноса. Цепочка переносов инвертируется каждый разряд, но для упрощения все названия `carry` не учитывают инверсию значений.
- res: результат выполнения логической операции или результат сумматора, который потом сохраняется на защёлке ADD. Результат операции в инвертированном виде.

Чтобы было понятней как получаются промежуточные результаты, на изображении ниже отмечены все основные мотивы:

![alu_bit_annotated_tran](/BreakingNESWiki/imgstore/6502/alu_bit_annotated_tran.jpg)

(Показан разряд 1, для остальных разрядов мотив выглядит аналогично)

Логическая схема для четных разрядов:

![alu_even_bit_logisim](/BreakingNESWiki/imgstore/logisim/alu_even_bit_logisim.jpg)

Логическая схема для нечетных разрядов:

![alu_odd_bit_logisim](/BreakingNESWiki/imgstore/logisim/alu_odd_bit_logisim.jpg)

Вычисление переполнения (контрольный сигнал `AVR`):

![alu_avr_tran](/BreakingNESWiki/imgstore/6502/alu_avr_tran.jpg)

![5_carry_plus_overflow_evaluation](/BreakingNESWiki/imgstore/6502/ttlworks/5_carry_plus_overflow_evaluation.png)

## Быстрый перенос BCD

Именно эта схема фигурирует в патенте US 3991307 (https://patents.google.com/patent/US3991307A).

![alu_bcd_carry_tran1](/BreakingNESWiki/imgstore/6502/alu_bcd_carry_tran1.jpg)

![alu_bcd_carry_tran2](/BreakingNESWiki/imgstore/6502/alu_bcd_carry_tran2.jpg)

Схемы для удобства восприятия "положены на бок".

Выход `DC3` подключается к цепочке переносов следующим образом:

![alu_carry3_tran](/BreakingNESWiki/imgstore/6502/alu_carry3_tran.jpg)

Как именно работает эта схема написано в патенте, мне добавить особо нечего. Просто месиво логических вентилей - делай так же и будет работать.

Кроме вычисления переноса для BCD схема также формирует контрольные сигналы `ACR` (перенос АЛУ для флагов) и `DAAH` для схемы BCD коррекции.

Логическая схема:

![alu_bcd_carry_logisim](/BreakingNESWiki/imgstore/logisim/alu_bcd_carry_logisim.jpg)

Оптимизированная логическая схема:

![3_alu_bcd_carry_logisim](/BreakingNESWiki/imgstore/6502/ttlworks/3_alu_bcd_carry_logisim.png)

## Промежуточный результат (ADD)

Промежуточный результат хранится на защёлке ADD (хранится в инвертированном виде, выдается на шины в прямом виде). Схема защёлки ADD состоит из 8 одинаковых кусков: 

![alu_add_tran](/BreakingNESWiki/imgstore/6502/alu_add_tran.jpg)

(Показана схема для разряда 0, остальные аналогично)

- ADD/SB06: Поместить значение защёлки ADD на шину SB. Для разряда 7 вместо ADD/SB06 используется контрольный сигнал `ADD/SB7`.
- ADD/ADL: Поместить значение защёлки ADD на шину ADL

## BCD Коррекция

Схема BCD коррекции управляется двумя сигналами: `/DAA` (выполнить коррекцию после сложения) и `/DSA` (выполнить коррекцию после вычитания).

Выходы схемы подключаются к входам аккумулятора (AC), при этом схема учитывает работу АЛУ, когда BCD режим отключен.

Часть входов аккумулятора подключена напрямую к шине SB и не участвуют в BCD коррекции (разряды 0 и 4).

В своей работе схема использует 4 вспомогательных внутренних сигнала: DAAL, DAAH, DSAL и DSAH. "L" в названии означает младшую часть разрядов (0-3), "H" - старшую часть разрядов (4-7).

Схемы для получения вспомогательных сигналов:

|DAAL|DSAL|DSAH|
|---|---|---|
|![alu_daal_tran](/BreakingNESWiki/imgstore/6502/alu_daal_tran.jpg)|![alu_dsal_tran](/BreakingNESWiki/imgstore/6502/alu_dsal_tran.jpg)|![alu_dsah_tran](/BreakingNESWiki/imgstore/6502/alu_dsah_tran.jpg)|

Схема `DAAH` находится в схеме переноса.

Непосредственно схемы коррекции используют общий мотив:
- На входе комбинаторные схемы, в разных комбинациях учитывающие 4 вспомогательных сигнала и разряды промежуточного результата (защёлки ADD)
- Выходной xor, одним из входов которого является разряд шины SB, а вторым указанные выше комбинаторные схемы

Распиленные схемы:

|Разряд 1|Разряд 2|Разряд 3|Разряд 5|Разряд 6|Разряд 7|
|---|---|---|---|---|---|
|![alu_bcd1_tran](/BreakingNESWiki/imgstore/6502/alu_bcd1_tran.jpg)|![alu_bcd2_tran](/BreakingNESWiki/imgstore/6502/alu_bcd2_tran.jpg)|![alu_bcd3_tran](/BreakingNESWiki/imgstore/6502/alu_bcd3_tran.jpg)|![alu_bcd5_tran](/BreakingNESWiki/imgstore/6502/alu_bcd5_tran.jpg)|![alu_bcd6_tran](/BreakingNESWiki/imgstore/6502/alu_bcd6_tran.jpg)|![alu_bcd7_tran](/BreakingNESWiki/imgstore/6502/alu_bcd7_tran.jpg)|

Причем вспомогательные сигналы /ADDx на схемах BCD коррекции получаются из значений разрядов защёлки ADD следующим образом:

![alu_add_temp_tran](/BreakingNESWiki/imgstore/6502/alu_add_temp_tran.jpg)

(На примере `/ADD5`)

Логическая схема:

![alu_bcd_logisim](/BreakingNESWiki/imgstore/logisim/alu_bcd_logisim.jpg)

Оптимизированная логическая схема:

![4_alu_bcd_logisim](/BreakingNESWiki/imgstore/6502/ttlworks/4_alu_bcd_logisim.png)

## Аккумулятор (AC)

Аккумулятор состоит из 8 одинаковых кусков:

![alu_ac_tran](/BreakingNESWiki/imgstore/6502/alu_ac_tran.jpg)

(Показана схема для разряда 3, остальные аналогично)

На вход аккумулятора поступает значение со схемы BCD коррекции (разряды 1-3, 5-7) или непосредственно с шины SB (разряды 0 и 4).

Кроме непосредственно выдачи аккумулятора на шины SB и DB в этом месте также выполняются другие операции с шинами, поэтому они рассматриваются также в этом разделе.

- SB/AC: Поместить значение с шины SB/схемы BCD коррекции в аккумулятор
- AC/SB: Поместить значение AC на шину SB
- AC/DB: Поместить значение AC на шину DB
- SB/DB: Соединить шину SB с шиной DB
- SB/ADH: Соединить шину SB с шиной ADH
- 0/ADH17: Принудительно записать 0 в разряды ADH\[1-7\]. Для разряда 0 вместо 0/ADH17 используется контрольный сигнал `0/ADH0`.
