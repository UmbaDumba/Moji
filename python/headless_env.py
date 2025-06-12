import cv2
import time
from ultralytics import YOLO

model_face = YOLO("Face_recognition/yolov8n-face.pt")
model_eye = YOLO("runs/detect/eye_tracking5/weights/best.pt")

cap = cv2.VideoCapture(0)
if not cap.isOpened():
    raise RuntimeError("웹캠을 열 수 없습니다.")

fourcc = cv2.VideoWriter_fourcc(*'XVID')
width, height = int(cap.get(3)), int(cap.get(4))
out = cv2.VideoWriter("result.avi", fourcc, fps, (width, height))

try:
    while True:
        ret, frame = cap.read()
        if not ret:
            break

        results_face = model_face(frame, conf=0.5)
        for face_box in results_face[0].boxes:
            x1, y1, x2, y2 = map(int, face_box.xyxy[0].cpu().numpy())
            face_crop = frame[y1:y2, x1:x2]

            results_eye = model_eye(face_crop, conf=0.4)
            for eye_box in results_eye[0].boxes:
                ex1, ey1, ex2, ey2 = map(int, eye_box.xyxy[0].cpu().numpy())
                ex1 += x1; ex2 += x1
                ey1 += y1; ey2 += y1

                label = model_eye.names[int(eye_box.cls)]
                cv2.rectangle(frame, (ex1, ey1), (ex2, ey2), (0, 0, 255), 2)
                cv2.putText(frame, label, (ex1, ey1 - 5), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255,0,0), 2)

            cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)

        out.write(frame)
        time.sleep(1 / fps)  # 실제 프레임 저장 속도 맞추기

except KeyboardInterrupt:
    print("중단됨")
finally:
    cap.release()
    out.release()

