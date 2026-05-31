import { useRef, useState } from "react";
import Tesseract from "tesseract.js";
import axiosClient from "../api/axiosClient";

function BarcodeScanner() {
  const videoRef = useRef<HTMLVideoElement | null>(null);
  const canvasRef = useRef<HTMLCanvasElement | null>(null);

  const [code, setCode] = useState("");
  const [error, setError] = useState("");
  const [foundProduct, setFoundProduct] = useState<any>(null);
  const [quantity, setQuantity] = useState("1");
  const [isReading, setIsReading] = useState(false);

  const startCameraScan = async () => {
    setError("");
    setCode("");

    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: "environment" },
      });

      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        await videoRef.current.play();
      }
    } catch (err) {
      console.error(err);
      setError("Could not access camera.");
    }
  };

  const stopCamera = () => {
    const stream = videoRef.current?.srcObject as MediaStream | null;

    if (stream) {
      stream.getTracks().forEach((track) => track.stop());
    }

    if (videoRef.current) {
      videoRef.current.srcObject = null;
    }
  };

  const extractNumbersFromImage = async (image: string | File) => {
    setIsReading(true);

    const result = await Tesseract.recognize(image, "eng");

    let numbers = result.data.text.replace(/\s/g, "").replace(/[^0-9]/g, "");

    if (numbers.length > 13) {
      numbers = numbers.slice(-13);
    }

    setIsReading(false);
    return numbers;
  };

  const captureAndReadNumbers = async () => {
    setError("");
    setCode("");

    if (!videoRef.current || !canvasRef.current) {
      setError("Camera is not ready.");
      return;
    }

    const canvas = canvasRef.current;
    const ctx = canvas.getContext("2d");

    if (!ctx) {
      setError("Could not capture image.");
      return;
    }

    canvas.width = videoRef.current.videoWidth;
    canvas.height = videoRef.current.videoHeight;

    ctx.drawImage(videoRef.current, 0, 0, canvas.width, canvas.height);

    try {
      const numbers = await extractNumbersFromImage(canvas.toDataURL("image/png"));

      if (numbers.length < 8) {
        setError("Could not detect barcode numbers. Try again.");
        return;
      }

      setCode(numbers);
    } catch (err) {
      console.error(err);
      setError("OCR failed.");
      setIsReading(false);
    }
  };

  const scanFromImage = async (event: React.ChangeEvent<HTMLInputElement>) => {
    setError("");
    setCode("");

    const file = event.target.files?.[0];
    if (!file) return;

    try {
      const numbers = await extractNumbersFromImage(file);

      if (numbers.length < 8) {
        setError("Could not detect barcode correctly.");
        return;
      }

      setCode(numbers);
    } catch (err) {
      console.error(err);
      setError("OCR failed.");
      setIsReading(false);
    }
  };

  const confirmCode = async () => {
    try {
      const cleanCode = code.trim().replace(/\D/g, "");

      const productResponse = await axiosClient.get(
        `/Products/by-code/${cleanCode}`
      );

      setFoundProduct(productResponse.data);
      setError("");
    } catch (error) {
      console.error(error);
      setError("Product not found. Check the barcode and try again.");
    }
  };

  const addExpense = async () => {
    if (!foundProduct) return;

    const qty = Number(quantity);

    if (qty <= 0) {
      setError("Quantity must be greater than 0.");
      return;
    }

    try {
      await axiosClient.post("/Transactions", {
        amount: foundProduct.unitPrice * qty,
        description: `${foundProduct.name} x${qty}`,
        date: new Date().toISOString(),
        categoryId: foundProduct.categoryId,
      });

      window.location.href = "/dashboard";
    } catch (error) {
      console.error(error);
      setError("Could not add expense.");
    }
  };

  return (
    <div className="scanner-page-modern">
      <section className="scanner-hero">
        <div>
          <span className="scanner-eyebrow">Smart product scanner</span>
          <h1>Scan a product barcode</h1>
          <p>
            Use your camera or upload a barcode image. SpendWise will find the
            product and add it as an expense.
          </p>
        </div>
      </section>

      <section className="scanner-layout">
        <div className="scanner-panel-modern">
          <div className="scanner-video-box">
            <video ref={videoRef} className="scanner-video-modern" />
            <canvas ref={canvasRef} style={{ display: "none" }} />

            <div className="scanner-overlay">
              <span />
            </div>
          </div>

          {error && <div className="scanner-error">{error}</div>}

          <div className="scanner-actions-modern">
            <button type="button" onClick={startCameraScan}>
              Start camera
            </button>

            <button type="button" onClick={captureAndReadNumbers}>
              {isReading ? "Reading..." : "Capture barcode"}
            </button>

            <button type="button" className="secondary-action" onClick={stopCamera}>
              Stop camera
            </button>
          </div>

          <label className="upload-box-modern">
            Upload barcode image
            <input type="file" accept="image/*" onChange={scanFromImage} hidden />
          </label>
        </div>

        <div className="scanner-result-panel">
          <h2>Scan result</h2>
          <p>Detected barcode numbers will appear here.</p>

          {code ? (
            <div className="result-box-modern">
              <span>Detected code</span>

              <input
                type="text"
                value={code}
                onChange={(e) => setCode(e.target.value.replace(/\D/g, ""))}
              />

              <small>Edit the number if OCR made a mistake.</small>

              <button type="button" onClick={confirmCode}>
                Confirm code
              </button>
            </div>
          ) : (
            <div className="empty-scan-state">
              No barcode detected yet.
            </div>
          )}

          {foundProduct && (
            <div className="product-found-card">
              <span>Product found</span>
              <h3>{foundProduct.name}</h3>

              <div className="product-info-row">
                <p>Price</p>
                <strong>{foundProduct.unitPrice} lei</strong>
              </div>

              <div className="product-info-row">
                <p>Category</p>
                <strong>{foundProduct.categoryName}</strong>
              </div>

              <input
                type="number"
                min="1"
                value={quantity}
                onChange={(e) => setQuantity(e.target.value)}
                placeholder="Quantity"
              />

              <button type="button" onClick={addExpense}>
                Add expense
              </button>
            </div>
          )}
        </div>
      </section>
    </div>
  );
}

export default BarcodeScanner;