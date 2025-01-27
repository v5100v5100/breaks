# H/V Счётчики

![ppu_locator_hv](/BreakingNESWiki/imgstore/ppu/ppu_locator_hv.jpg)

H/V-счётчики считают количество пикселей в строке и количество строк, соответственно. H тикает от 0 до 340 (включая), V тикает от 0 до 261 (включая). Итого видимая и невидимая часть экрана представляет собой 262 строки по 341 пикселя каждая.

Технически счётчики состоят из 9 разрядов, поэтому могут считать от 0 до 0x1FF, но они никогда не считают полностью и ограничиваются максимальными значениями H и V. Для этого схема H/V FSM периодически сбрасывает их.

## Стадия счётчика

Рассмотрим работу одной стадии (разряда) счётчика на примере V-Counter.

![HV_stage](/BreakingNESWiki/imgstore/ppu/HV_stage.jpg)

![hv_stage2](/BreakingNESWiki/imgstore/ppu/hv_stage2.jpg)

![hv_stage2_annotated](/BreakingNESWiki/imgstore/ppu/hv_stage2_annotated.jpg)

- `carry_in`: входной перенос
- `carry_out`: выходной перенос
- `out`: выход одного разряда счётчика
- `/out`: выход одного разряда счётчика (инвертированное значение). Используется в схемах оптимизации переноса.
- `VC` (или `HC` у H-Counter): сигнал очистки всего счётчика. Данный способ очистки используется для управления очисткой счётчиков со стороны схемы H/V FSM.
- `RES`: сигнал общего сброса. Это глобальный сигнал сброса всех последовательностных схем PPU.
- `PCLK`: Pixel Clock

На изображении выделены транзисторы, которые формируют логические элементы.

Схема не очень сложная, за исключением необычной организации FF на базе двух 2-nor и двух мультиплексоров, которые формируют петлю FF.

Логическая схема:

![hv_stage_logisim](/BreakingNESWiki/imgstore/ppu/hv_stage_logisim.jpg)

Смысл заключается в следующем:
- Виртуально текущее значение FF можно представить как выход мультиплексора, управляемого PCLK
- В состоянии PCLK=0 значение FF регенерируется старым значением (с учетом глобального сброса `RES`). Также при этом задействуется схема пересчёта, которая выполнена на мультиплексоре Carry и статической защелке
- В состоянии PCLK=1 значение FF обновляется новым значением со схемы пересчёта

Возможно тут на самом деле прячется какой-то JK-триггер, но я в этих JK не разбираюсь. На Вики есть большая статья о счётчиках, кому интересно почитайте.

## Устройство H/V счётчиков

|H|V|
|---|---|
|![H_trans](/BreakingNESWiki/imgstore/ppu/H_trans.jpg)|![V_trans](/BreakingNESWiki/imgstore/ppu/V_trans.jpg)|

- HCounter всегда считает, т.к. carry_in 0-го разряда всегда равен 1 (соединен с Vdd)
- VCounter увеличивает значение на 1 только когда активен вход `V_IN`
- Выходной перенос каждого предыдущего бита заведен на входной перенос следующего, для формирования carry-chain
- Каждый счётчик включает в себя дополнительную логику переноса (далее), по этой причине и пришлось выложить большие картинки счётчиков, чтобы было видно эту логику

## Дополнительная логика переноса

Счётчики включают в свой состав следующие небольшие схемы:

|HCounter|VCounter|
|---|---|
|![CARRYH](/BreakingNESWiki/imgstore/ppu/CARRYH.jpg)|![CARRYV](/BreakingNESWiki/imgstore/ppu/CARRYV.jpg)|
|![CarryH_Logic](/BreakingNESWiki/imgstore/ppu/CarryH_Logic.jpg)|![CarryV_Logic](/BreakingNESWiki/imgstore/ppu/CarryV_Logic.jpg)|

Схема для HCounter не включает аналога V_IN, так как входной перенос для H всегда равен 1 и не требуется для операции NOR, которую представляет эта дополнительная логика.

Зачем она нужна? Скорее всего, чтобы уменьшить propagation delay цепочки переносов.

При симуляции можно вполне обойтись без этих "патчей", работа счётчиков не будет отличаться.
