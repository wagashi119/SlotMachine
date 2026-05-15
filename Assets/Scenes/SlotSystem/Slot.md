# SlotSystemの構成


- SlotReel:リールスデータを保持するクラス。
	- Reel: リールス
	- GetImage(vector2) : 引数がスロットの座標
	- GetScore(vector2)　: 引数がスロットの座標
	- GetSlotCount() : リールスのリール量と、リール配列の量をint[]で返す

- Select :指定されたリールから、ランダムにセレクト
	- SlotReel : 元データ
	- SlotView : 描画クラス


	- Select()
		- 返り値を持つ
		- リールからランダムに選択

- SlotView
	- SlotView
	- offset: スロット演出
	- y-interva :配置する画像の間隔

	- rollTime : ロールする時間
	- rollTimeCout : ロール時間
	- rollSpeed: ロールするスピード




## リールデータ
- リールス :スロット分のリール配列をまとめたクラス
	- ScriptableObject
- リール
	- 画像と報酬、とか
	- ScriptableObject?