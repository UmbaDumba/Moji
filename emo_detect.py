from ultralytics import YOLO
import cv2
import numpy as np
from facenet_pytorch import MTCNN

# 모델 초기화
model = YOLO("yolov8n-seg.pt")
mtcnn = MTCNN(keep_all=True)

# 웹캠 열기
cap = cv2.VideoCapture(1)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    results = model(frame)
    annotated = frame.copy()

    for r in results:
        if r.masks is not None:
            classes = r.boxes.cls.cpu().numpy().astype(int)
            masks = r.masks.xy

            for cls, seg, box in zip(classes, masks, r.boxes.xyxy):
                if cls == 0:  # 사람
                    # YOLO 윤곽선 표시
                    pts = np.array(seg, dtype=np.int32)
                    cv2.polylines(annotated, [pts], isClosed=True, color=(0, 255, 0), thickness=2)

                    # 사람 영역만 잘라서 얼굴 검출
                    x1, y1, x2, y2 = map(int, box)
                    person_roi = frame[y1:y2, x1:x2]
                    faces, _ = mtcnn.detect(person_roi)

                    if faces is not None:
                        for fx1, fy1, fx2, fy2 in faces:
                            fx1, fy1, fx2, fy2 = map(int, [fx1, fy1, fx2, fy2])
                            # 전체 프레임 기준 얼굴 위치 보정
                            cv2.rectangle(annotated, (x1+fx1, y1+fy1), (x1+fx2, y1+fy2), (0, 255, 255), 2)
                            cv2.putText(annotated, "Face", (x1+fx1, y1+fy1-10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 255), 2)

    cv2.imshow("YOLO + Face Detection", annotated)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
