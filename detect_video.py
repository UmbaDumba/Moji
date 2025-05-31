from ultralytics import YOLO
import cv2

# 학습된 모델 로드
model = YOLO("runs/eye_yolo_train/weights/best.pt")

# 웹캠 열기
cap = cv2.VideoCapture(1)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # YOLOv8 감지
    results = model(frame)

    # 시각화 결과 출력
    annotated_frame = results[0].plot()
    cv2.imshow("YOLOv8 Detection", annotated_frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
