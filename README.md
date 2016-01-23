# Saving and loading system
A good programmer is a lazy programmer. That's why I made several small tools to keep reusing for the games I make with Wrecking Koala. On of these systems is a saving & loading system. It automatically serializes classes to XML for me, and then stores them on the device by use of encryption.

Currently every class that implements iProgressData has to implement their own save and load functionality. It feels redundant to do so, because it's the same every time I implement it. For the next Wrecking Koala project I will improve on this.